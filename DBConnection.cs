using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;

namespace KeywordGetherer
{
    class DBConection
    {
        private static Mutex dbMutext = new Mutex();

        public class DBConnectionException : Exception
        {

        }

        public class DBKeyword
        {
            public string keyword { get; set; }
            public int keyword_id { get; set; }
        }
        private MySqlConnection conn;

        public DBConection()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            string connStr = ConfigurationManager
                .ConnectionStrings["keywordsConnStr"]
                .ConnectionString;
            conn = new MySqlConnection(connStr);
        }

        public int getKeywordId(String keyword)
        {
            if (!this.OpenConnection())
                throw new DBConnectionException();

            int keyword_id = -1;


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
                    keyword_id = dataReader.GetInt32("id");
                }
         
                dataReader.Close();
                this.CloseConnection();
                return keyword_id ;

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                return -1;
            }
        }
        public bool isKeywordExist(String keyword)
        {
            if (!this.OpenConnection())
                throw new DBConnectionException();

            int keyword_id = -1;


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
                    keyword_id = dataReader.GetInt32("id");
                }
                Console.WriteLine("keyword=>" + keyword + " id=>" + (keyword_id == -1 ? "нет в бд" : "" + keyword_id));
                dataReader.Close();
                this.CloseConnection();
                return (keyword_id == -1?false:true);

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                return true;
            }

            
        }
        public void Insert(String keyword)
        {

            if (!this.OpenConnection())
                throw new DBConnectionException();


            try
            {
                string replacement = "";
                Regex rgx = new Regex("['\"]");

                string query = "INSERT INTO `keywords` " +
                    "(`keyword`, `created_at`, `updated_at`) VALUES " +
                    "(@keyword_kw,@created_at,@updated_at)";


                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword_kw", rgx.Replace(keyword, replacement));
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //Console.WriteLine("Ошибочка:" + e);
            }
            this.CloseConnection();

        }

        public void InsertForecast(Forecastinfo forecast)
        {
            //return;
            if (!this.OpenConnection())
                throw new DBConnectionException();


            try
            {
         
                string query = "INSERT INTO `forecastinfo` " +
                    "(`min`,`max`,`premium_min`,`premium_max`,`shows`,`clicks`,`first_place_clicks`,`premium_clicks`,`ctr`,`first_place_ctr`,`premium_ctr`,`currency`,`Keywords_id`,`is_preceded` ,`created_at`, `updated_at`) VALUES " +
                    "(@min,@max,@premium_min,@premium_max,@shows,@clicks,@first_place_clicks,@premium_clicks,@ctr,@first_place_ctr,@premium_ctr,@currency,@Keywords_id,@is_preceded ,@created_at, @updated_at)";


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
            }
            catch (Exception e)
            {
                //Console.WriteLine("Ошибочка:" + e);
            }
            this.CloseConnection();

        }


        public List<DBKeyword> listKeywords(long offset, int limit)
        {
            //List<DBKeyword> l = new List<DBKeyword>();
            //for (int i = 0; i < limit; i++) {
            //    DBKeyword kw = new DBKeyword();
            //    kw.keyword = "окна";
            //    l.Add(kw);
            //}
            //return l;
            if (!this.OpenConnection())
                return new List<DBKeyword>();

            //выбирае из бд инфу с определенным смещением, чтоб не нагружать оперативку
            string query = "SELECT `keyword`, `id` FROM `keywords`  ORDER BY `id` asc LIMIT @limit OFFSET @offset ";
            List<DBKeyword> list_kw = new List<DBKeyword>();

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
                dbkw.keyword_id = Int32.Parse("" + dataReader["id"]);
                list_kw.Add(dbkw);
            }

            dataReader.Close();
            this.CloseConnection();

            return list_kw;

        }

        public long countKewyrods()
        {
            if (!this.OpenConnection())
                return -1;

            string query = "SELECT Count(*) FROM `keywords`";
            long Count = -1;

            MySqlCommand cmd = new MySqlCommand(query, conn);
            Count = (long)(cmd.ExecuteScalar());
            this.CloseConnection();

            return Count;
        }

        public List<DBKeyword> wordsForReport(int offset,int limit)
        {
            if (!this.OpenConnection())
                return new List<DBKeyword>();

            //выбирае из бд инфу с определенным смещением, чтоб не нагружать оперативку
            string query = "select t1.*from keywords as t1 where t1.id not in (select t2.Keywords_id from forecastinfo t2) LIMIT @limit OFFSET @offset";
            List<DBKeyword> list_kw = new List<DBKeyword>();

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
                dbkw.keyword_id = Int32.Parse("" + dataReader["id"]);
                list_kw.Add(dbkw);
            }

            dataReader.Close();
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
    }
}
