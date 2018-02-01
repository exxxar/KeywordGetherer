using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Text.RegularExpressions;

namespace KeywordGetherer
{
    class Program
    {
        public const int STEP = 5;


        static void Main(string[] args)
        {
            foreach(string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                if (file.IndexOf(".csv") != -1)  
                    File.Delete(file);

            int offset = 0;
            int limit = STEP;

            var MyIni = new IniFiles("Settings.ini");

            offset = !MyIni.KeyExists("offset") ?
               Int32.Parse(MyIni.Write("offset" ,""+ offset)) :
                Int32.Parse(MyIni.Read("offset"));

            limit = !MyIni.KeyExists("limit" ) ?
               Int32.Parse(MyIni.Write("limit" , "" + STEP)) :
               Int32.Parse(MyIni.Read("limit"));

            DBConection db = new DBConection();

            foreach (DBConection.DBKeyword kw in db.list(offset,limit))
            {
                var options = new ChromeOptions();
                //options.AddArgument("no-sandbox");
                options.AddUserProfilePreference("download.default_directory", Directory.GetCurrentDirectory());
                options.AddArguments("--disable-extensions");
                // options.AddArgument("no-sandbox");
                options.AddArgument("--incognito");
                // options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");  //--disable-media-session-api
                                                       //options.AddArgument("--remote-debugging-port=9222");

                ChromeDriver driver = new ChromeDriver(options);//открываем сам браузер

                driver.LocationContext.PhysicalLocation = new OpenQA.Selenium.Html5.Location(55.751244, 37.618423, 152);


                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //время ожидания компонента страницы после загрузки страницы
                driver.Manage().Cookies.DeleteAllCookies();

                driver.Navigate().GoToUrl("http://www.bukvarix.com/keywords/?q="+kw.keyword+"&r=report");

                IWebElement element = driver.FindElement(By.CssSelector(".report-download-button"));
                string fileName = element.GetAttribute("download");
                Console.WriteLine("Скачиваем:" + fileName);


                element.SendKeys(Keys.Enter);
                while (!File.Exists(fileName))
                {
                    System.Threading.Thread.Sleep(2000);
                }

                string[] keywords = File.ReadAllLines(fileName);
                int index = 0;
                foreach (string s in keywords)
                {
                    if (index > 0)
                    {
                        string[] buf = s.Split(';');

                        string replacement = "";
                        Regex rgx = new Regex("['\"]");
                        string keyword = rgx.Replace(buf[0], replacement);
                     
                        if (int.Parse(buf[1]) <= 7)
                        {
                          
                            try
                            {
                                if (db.isKeywordExist(keyword) == -1)
                                    db.Insert(keyword);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }

                    }
                    index++;
                }
                File.Delete(fileName);

                offset += STEP;
                MyIni.Write("offset", "" + offset);
                driver.Close();
            }
        }
    }
}
