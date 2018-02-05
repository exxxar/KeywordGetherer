using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Yandex.Direct.Configuration;
using Yandex.Direct.Authentication;
using Yandex.Direct;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class Program
    {
        static void Main(string[] args)
        {

            KwGetherer kwGetherer = new KwGetherer();
            
            Task.Run(() => kwGetherer.execute());

            Console.ReadKey();
            return;
            //YandexDirect yandexDirect = new YandexDirect("azyexxxar", "906fe2dd055a4f6fa08b0765156e6cf7", "AQAAAAAhzCtcAASm9aZlZ6gwzUielkMitN3PEsc");
            //yandexDirect.execute(new string[] { "холод" }, new int[] { 1 });
            //Console.ReadLine();
         

            //return;
            //foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
            //    if (file.IndexOf(".csv") != -1)
            //        File.Delete(file);

            //int offset = 0;
            //int limit = STEP;
            //int file_offset = 0;

            //var MyIni = new IniFiles("Settings.ini");

            //offset = !MyIni.KeyExists("offset") ?
            //   Int32.Parse(MyIni.Write("offset", "" + offset)) :
            //    Int32.Parse(MyIni.Read("offset"));

            //limit = !MyIni.KeyExists("limit") ?
            //   Int32.Parse(MyIni.Write("limit", "" + STEP)) :
            //   Int32.Parse(MyIni.Read("limit"));

            //bool loadFromFile = !MyIni.KeyExists("loadFromFile") ?
            //  Boolean.Parse(MyIni.Write("loadFromFile", "false")) :
            //  Boolean.Parse(MyIni.Read("loadFromFile"));

            //string loadFromPath = !MyIni.KeyExists("loadFromPath") ?
            //    MyIni.Write("loadFromPath", "words.txt") :
            //    MyIni.Read("loadFromPath");

            //List<DBConection.DBKeyword> fileData = new List<DBConection.DBKeyword>();

            //if (loadFromFile)
            //{
            //    offset = !MyIni.KeyExists("file_offset") ?
            //       Int32.Parse(MyIni.Write("file_offset", "" + file_offset)) :
            //        Int32.Parse(MyIni.Read("file_offset"));
            //    fileData =  loadFile(loadFromPath);

            //}


            //DBConection db = new DBConection();
            
            
            //int count = loadFromFile ? fileData.Count : db.count();

            //Console.WriteLine("Всего элементов " + count);
            //while (offset < count)
            //{
            //    if (loadFromFile && offset >= count)
            //    {
            //        MyIni.Write("loadFromFile", "false");
            //        loadFromFile = false;
            //        count = db.count();
            //        Console.WriteLine("Переключаемся на выборку из БД");
            //    }
            //    List<DBConection.DBKeyword> list = loadFromFile ? fileData : db.list(offset, limit);

            //    foreach (DBConection.DBKeyword kw in list)
            //    {
            //        var options = new ChromeOptions();
            //        //options.AddArgument("no-sandbox");
            //        options.AddUserProfilePreference("download.default_directory", Directory.GetCurrentDirectory());
            //        options.AddArguments("--disable-extensions");
            //        // options.AddArgument("no-sandbox");
            //        options.AddArgument("--incognito");
            //        // options.AddArgument("--headless");
            //        options.AddArgument("--disable-gpu");  //--disable-media-session-api
            //                                               //options.AddArgument("--remote-debugging-port=9222");

            //        ChromeDriver driver = new ChromeDriver(options);//открываем сам браузер
            //        driver.LocationContext.PhysicalLocation = new OpenQA.Selenium.Html5.Location(55.751244, 37.618423, 152);
            //        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //время ожидания компонента страницы после загрузки страницы
            //        driver.Manage().Cookies.DeleteAllCookies();

            //        driver.Navigate().GoToUrl("http://www.bukvarix.com/keywords/?q=" + kw.keyword + "&r=report");
            //        if (!driver.isSelectorExist(By.CssSelector(".report-download-button")))
            //        {
            //            driver.Close();
            //            continue;
            //        }

            //        IWebElement element = driver.FindElement(By.CssSelector(".report-download-button"));
            //        string fileName = element.GetAttribute("download");
            //        Console.WriteLine("Скачиваем:" + fileName);

            //        element.SendKeys(Keys.Enter);
            //        while (!File.Exists(fileName))
            //        {
            //            System.Threading.Thread.Sleep(2000);
            //        }

            //        driver.Close();
            //        string[] keywords = File.ReadAllLines(fileName);
            //        int index = 0;
            //        foreach (string s in keywords)
            //        {
            //            if (index > 0)
            //            {
            //                string[] buf = s.Split(';');

            //                string replacement = "";
            //                Regex rgx = new Regex("['\"]");
            //                string keyword = rgx.Replace(buf[0], replacement);

            //                if (int.Parse(buf[1]) <= 7 && int.Parse(buf[4]) <= 100)
            //                {
            //                    try
            //                    {
            //                        if (db.isKeywordExist(keyword) == -1)
            //                            db.Insert(keyword);
            //                    }
            //                    catch (Exception e)
            //                    {
            //                        Console.WriteLine(e.Message);
            //                    }
            //                }
            //            }
            //            index++;
            //        }
            //        File.Delete(fileName);
            //    }

            //    offset += STEP;
            //    MyIni.Write(loadFromFile ? "file_offset" : "offset", "" + offset);
            //}
        }

        //public static List<DBConection.DBKeyword> loadFile(String path)
        //{
        //    List<DBConection.DBKeyword> list = new List<DBConection.DBKeyword>();
        //    string[] buf = File.ReadAllLines(path);
        //    foreach (string s in buf)
        //    {
        //        DBConection.DBKeyword dbw = new DBConection.DBKeyword();
        //        dbw.keyword = s;
        //        list.Add(dbw);
        //    }
        //    return list;
        //}
    }
}
