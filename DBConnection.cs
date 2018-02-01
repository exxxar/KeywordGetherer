using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;


namespace KeywordGetherer
{
    class DBConection
    {
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

       
        public int isKeywordExist(String keyword)
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
                Console.WriteLine("keyword=>" + keyword + " id=>" + (keyword_id==-1?"нет в бд":""+keyword_id));
                dataReader.Close();
                this.CloseConnection();
                return keyword_id;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return keyword_id;
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
                Console.WriteLine("Ошибочка:"+e);
            }
            this.CloseConnection();
        }


        public List<DBKeyword> list(int offset, int limit)
        {
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

        public int count()
        {
            if (!this.OpenConnection())
                return -1;

            string query = "SELECT Count(*) FROM `keywords`";
            int Count = -1;

            MySqlCommand cmd = new MySqlCommand(query, conn);
            Count = int.Parse(cmd.ExecuteScalar() + "");
            this.CloseConnection();

            return Count;

        }

        private bool CloseConnection()
        {
            try
            {
                conn.Close();
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
                return false;
            }
        }
    }
}
