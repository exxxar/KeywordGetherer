using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using KeywordGetherer.SiteParser;

namespace KeywordGetherer
{
    public class DBConection:DBUtils
    {
        private static Mutex dbMutext = new Mutex();

        public class DBConnectionException : Exception
        {

        }

        public class DBKeyword
        {
            public string keyword { get; set; }
            public long keyword_id { get; set; }
        }
        private MySqlConnection conn;

        public DBConection()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            try
            {
                string connStr = ConfigurationManager
                    .ConnectionStrings["keywordsConnStr"]
                    .ConnectionString;
                conn = new MySqlConnection(connStr);
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public long getKeywordId(String keyword)
        {
            if (!this.OpenConnection())
                throw new DBConnectionException();

            long keyword_id = -1;
            
            try
            {
                string replacement = "";
                Regex rgx = new Regex("['\"]");

                string query = "SELECT * FROM `keywords` WHERE `keyword`=@keyword;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword", rgx.Replace(keyword, replacement));
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    keyword_id = dataReader.GetInt64("id");
                }

                dataReader.Close();
                this.CloseConnection();
                return keyword_id;

            }
            catch 
            {
                return -1;
            }
        }
        public bool isKeywordExist(String keyword)
        {
            if (!this.OpenConnection())
                throw new DBConnectionException();

            long keyword_id = -1;

            string replacement = "";
            Regex rgx = new Regex("['\"]");

            string query = "SELECT * FROM `keywords` WHERE `keyword`=@keyword;";

            try
            { 
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword", rgx.Replace(keyword, replacement));
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    keyword_id = dataReader.GetInt64("id");
                }
                Console.WriteLine("keyword=>" + keyword + " id=>" + (keyword_id == -1 ? "нет в бд" : "" + keyword_id));
                dataReader.Close();
                this.CloseConnection();
                return (keyword_id == -1 ? false : true);
            }
            catch 
            {
                return true;
            }


        }

        public Dictionary<string,long> getTablesCountInfo()
        {
            if (!this.OpenConnection())
                throw new DBConnectionException();            

           string query = "SELECT table_name, table_rows FROM information_schema.tables where table_schema = 'keywords'; ";
            Dictionary<string, long> buf = new Dictionary<string, long>();
            try
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, conn);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    this.CloseConnection();
                    return null;
                }
                //Read the data and store them in the list
                while(dataReader.Read())
                    buf.Add(""+dataReader["table_name"],Int64.Parse("" + dataReader["table_rows"]));                

                dataReader.Close();
            }
            catch { }
            this.CloseConnection();

            return buf;
        }
        public void Insert(String keyword)
        {

            if (!this.OpenConnection())
                throw new DBConnectionException();

            string replacement = "";
            Regex rgx = new Regex("['\"]");

            string query = "INSERT INTO `keywords` " +
                "(`keyword`, `created_at`, `updated_at`) VALUES " +
                "(@keyword_kw,@created_at,@updated_at)";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword_kw", rgx.Replace(keyword, replacement));
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            catch { }
            this.CloseConnection();
        }

        public void InsertTEST(String keyword)
        {

            if (!this.OpenConnection())
                throw new DBConnectionException();

            string replacement = "";
            Regex rgx = new Regex("['\"]");

            string query = "INSERT INTO `keywords` " +
                "(`keyword`, `created_at`, `updated_at`) VALUES " +
                "(@keyword_kw,@created_at,@updated_at)";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword_kw", rgx.Replace(keyword, replacement));
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            catch { }
            this.CloseConnection();
        }

        public long InsertForecast(Forecastinfo forecast)
        {
            //return;
            if (!this.OpenConnection())
                throw new DBConnectionException();

            string query = "INSERT INTO `forecastinfo` " +
                 "(`min`,`max`,`premium_min`,`premium_max`,`shows`,`clicks`,`first_place_clicks`,`premium_clicks`,`ctr`,`first_place_ctr`,`premium_ctr`,`currency`,`Keywords_id`,`is_preceded` ,`created_at`, `updated_at`) VALUES " +
                 "(@min,@max,@premium_min,@premium_max,@shows,@clicks,@first_place_clicks,@premium_clicks,@ctr,@first_place_ctr,@premium_ctr,@currency,@Keywords_id,@is_preceded ,@created_at, @updated_at)";


            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@min", forecast.Min);
                cmd.Parameters.AddWithValue("@max", forecast.Max);
                cmd.Parameters.AddWithValue("@premium_min", forecast.PremiumMin);
                cmd.Parameters.AddWithValue("@premium_max", forecast.PremiumMax);
                cmd.Parameters.AddWithValue("@shows", forecast.Shows);
                cmd.Parameters.AddWithValue("@clicks", forecast.Shows);
                cmd.Parameters.AddWithValue("@first_place_clicks", forecast.FirstPlaceClicks);
                cmd.Parameters.AddWithValue("@premium_clicks", forecast.PremiumClicks);
                cmd.Parameters.AddWithValue("@ctr", forecast.CTR);
                cmd.Parameters.AddWithValue("@first_place_ctr", forecast.FirstPlaceCtr);
                cmd.Parameters.AddWithValue("@premium_ctr", forecast.PremiumCtr);
                cmd.Parameters.AddWithValue("@currency", forecast.Currency);
                cmd.Parameters.AddWithValue("@Keywords_id", forecast.Keyword_id);
                cmd.Parameters.AddWithValue("@is_preceded", forecast.is_preceded);
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);
                cmd.ExecuteNonQuery();
                long lastId = (long)cmd.LastInsertedId;
                this.CloseConnection();
                return lastId;
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибочка InsertForecast:" + e);
            }
            this.CloseConnection();
            return -1;
        }

        public void InsertAuctionBids(AuctionBids auctionBids)
        {
            if (!this.OpenConnection())
                throw new DBConnectionException();

            try
            {

                string query = "INSERT INTO `auctionbids` " +
                    "(`position`,`bid`,`price`,`forecastInfo_id`,`created_at`, `updated_at`) VALUES " +
                    "(@position,@bid,@price,@forecastInfo_id,@created_at, @updated_at)";

                long pos = 0;


                switch (auctionBids.Position.ToUpper())
                {
                    case "P11": pos = 1; break;
                    case "P12": pos = 2; break;
                    case "P13": pos = 3; break;
                    case "P14": pos = 4; break;
                    case "P21": pos = 5; break;
                    case "P22": pos = 6; break;
                    case "P23": pos = 7; break;
                    case "P24": pos = 8; break;
                }

                Console.WriteLine("auctionBids=>" + auctionBids.ToString());
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@position", pos);
                cmd.Parameters.AddWithValue("@bid", auctionBids.Bid);
                cmd.Parameters.AddWithValue("@price", auctionBids.Price);
                cmd.Parameters.AddWithValue("@forecastInfo_id", auctionBids.forecastInfo_id);
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибочка InsertAuctionBids:" + e);
            }
            this.CloseConnection();

        }

        public List<string> listSites(long offset, int limit)
        {

            if (!this.OpenConnection())
                return new List<string>();

            List<string> list_sites = new List<string>();
            //выбирае из бд инфу с определенным смещением, чтоб не нагружать оперативку
            string query = "SELECT * FROM `site`  ORDER BY `site_id` desc LIMIT @limit OFFSET @offset ";

            try
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@offset", offset);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    this.CloseConnection();
                    return null;
                }
                //Read the data and store them in the list
                while (dataReader.Read())
                    list_sites.Add("" + dataReader["site"]);

                dataReader.Close();
            }
            catch
            {

            }
            this.CloseConnection();

            return list_sites;
        }
        public List<DBKeyword> listKeywords(long offset, int limit)
        {

            if (!this.OpenConnection())
                return new List<DBKeyword>();

            //выбирае из бд инфу с определенным смещением, чтоб не нагружать оперативку
            string query = "SELECT `keyword`, `id` FROM `keywords`  ORDER BY `id` asc LIMIT @limit OFFSET @offset ";
            List<DBKeyword> list_kw = new List<DBKeyword>();

            try
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@offset", offset);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    this.CloseConnection();
                    return null;
                }
                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    DBKeyword dbkw = new DBKeyword();
                    dbkw.keyword = "" + dataReader["keyword"];
                    dbkw.keyword_id = Int64.Parse("" + dataReader["id"]);
                    list_kw.Add(dbkw);
                }

                dataReader.Close();
            }
            catch { }

            this.CloseConnection();

            return list_kw;

        }

     
        public long countKewyrods()
        {
            if (!this.OpenConnection())
                return -1;

            string query = "SELECT Count(*) FROM `keywords`";
            long Count = -1;
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Count = (long)(cmd.ExecuteScalar());
            }
            catch { }
            this.CloseConnection();
            return Count;
        }

        public List<DBKeyword> wordsForReport(long offset, int limit)
        {
            if (!this.OpenConnection())
                return new List<DBKeyword>();

            //выбирае из бд инфу с определенным смещением, чтоб не нагружать оперативку
            string query = "select t1.*from keywords as t1 where t1.id not in (select t2.Keywords_id from forecastinfo t2) LIMIT @limit OFFSET @offset";
            List<DBKeyword> list_kw = new List<DBKeyword>();
            try
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@offset", offset);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    this.CloseConnection();
                    return null;
                }
                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    DBKeyword dbkw = new DBKeyword();
                    dbkw.keyword = "" + dataReader["keyword"];
                    dbkw.keyword_id = Int64.Parse("" + dataReader["id"]);
                    list_kw.Add(dbkw);
                }

                dataReader.Close();
            }
            catch { }

            this.CloseConnection();

            return list_kw;
        }



        private bool CloseConnection()
        {

            try
            {
                conn.Close();
                dbMutext.ReleaseMutex();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                dbMutext.WaitOne();
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {

                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                this.CloseConnection();
                return false;
            }

        }


        public Boolean isExist_AdSearchPosition(Keyword keyword)
        {
            if (!this.OpenConnection())
                throw new DBConnectionException();

            Console.WriteLine("Проверяем в бд [" + keyword.toString() + "]");

            long Count = -1;

            try
            {
                string replacement = "";
                Regex rgx = new Regex("['\"]");

                string query = "SELECT Count(*) as count FROM `adsearchpostions` WHERE `Keywords_id`=@Keywords_id and `search_engine`=@search_engine and `positions`=@positions and `description`=\"@description\" and 	`AdSearchPostions_site_id`=@AdSearchPostions_site_id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Keywords_id", keyword.keyword_id);
                cmd.Parameters.AddWithValue("@search_engine", keyword.search_engine);
                cmd.Parameters.AddWithValue("@positions", keyword.position);
                cmd.Parameters.AddWithValue("@description", rgx.Replace(keyword.description, replacement));
                cmd.Parameters.AddWithValue("@AdSearchPostions_site_id", keyword.site_id);

                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Count = dataReader.GetInt64("count");

                    //if (dataReader.FieldCount>1) { 
                    //    Console.WriteLine(dataReader.GetString("position") != null ? "ПОЗИЦИЯ:" + dataReader.GetString("position") : "позиции нет");
                    //    Console.WriteLine(dataReader.GetString("description") != null ? "ОПИСАНИЕ:" + dataReader.GetString("description") : "описания нет");
                    // }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.CloseConnection();
            Console.WriteLine("Кол-во в БД!![" + Count + "]");
            return Count > 0;
        }

        public void Insert_AdSearchPosition(Keyword keyword)
        {

            if (!this.OpenConnection())
                throw new DBConnectionException();

            try
            {
                string replacement = "";
                Regex rgx = new Regex("['\"]");

                Console.WriteLine("Добавляем в бд [" + keyword.toString() + "]");
                string query = "INSERT INTO `adsearchpostions` " +
                    "(`AdSearchPostions_site_id`, `description`, `positions`, `search_engine`, `Keywords_id`,`created_at`,`updated_at`,`is_ad`,`region_id`) VALUES " +
                    "(@AdSearchPostions_site_id,@description,@positions,@search_engine,@Keywords_id,@created_at,@updated_at,@is_ad,@region_id)";


                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AdSearchPostions_site_id", keyword.site_id);
                cmd.Parameters.AddWithValue("@description", rgx.Replace(keyword.description, replacement));
                cmd.Parameters.AddWithValue("@positions", keyword.position);
                cmd.Parameters.AddWithValue("@search_engine", keyword.search_engine);
                cmd.Parameters.AddWithValue("@Keywords_id", keyword.keyword_id);
                cmd.Parameters.AddWithValue("@created_at", keyword.created_at);
                cmd.Parameters.AddWithValue("@updated_at", keyword.updated_at);
                cmd.Parameters.AddWithValue("@is_ad", keyword.is_ad);
                cmd.Parameters.AddWithValue("@region_id", keyword.region_id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.CloseConnection();
        }



        public long isUrlExist(String site_url)
        {
            if (!this.OpenConnection())
                return -1;

            long site_id = -1;

            try
            {

                string query = "SELECT * FROM `site` WHERE `site`=@site_url;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@site_url", site_url);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    site_id = dataReader.GetInt64("site_id");
                }

                dataReader.Close();
                this.CloseConnection();
                return site_id;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return site_id;
        }

        public long Insert_Site(String site_url)
        {
            if (!this.OpenConnection())
                return -1;

            Console.WriteLine("Добавляем URL в бд " + site_url);
            MySqlCommand cmd = null;
            //create command and assign the query and connection from the constructor
            try
            {
                string query = "INSERT INTO `site` (`site`) VALUES ( @site_url );";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@site_url", site_url);
                cmd.ExecuteNonQuery();
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.CloseConnection();
            return cmd.LastInsertedId;
        }

        public void Insert_Uri(String site_uri)
        {
            if (!this.OpenConnection())
                return;

            //create command and assign the query and connection from the constructor
            try
            {
                string query = "INSERT INTO `uri` (`uri`,`site_id`) VALUES (@uri,@site_id);";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uri", site_uri);
                cmd.Parameters.AddWithValue("@site_id", site_uri);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.CloseConnection();
        }

        public long Select_Uri_id(String uri)
        {

            if (!this.OpenConnection())
                return -1;

            long uri_id = -1;

            try
            {
                //Create Command
                string query = "SELECT uri_id FROM `uri` WHERE `uri`=@uri";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@uri", uri);

                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    uri_id = dataReader.GetInt64("uri_id");
                }
                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.CloseConnection();

            return uri_id;
        }


        public long Select_Site_id(String site_url)
        {

            if (!this.OpenConnection())
                return -1;

            long site_id = -1;

            try
            {
                //Create Command
                string query = "SELECT site_id FROM `site` WHERE `site`=@site_url";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@site_url", site_url);

                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    site_id = dataReader.GetInt64("site_id");
                }
                dataReader.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.CloseConnection();

            return site_id;
        }


    }
}
