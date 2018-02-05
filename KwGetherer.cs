using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class KwGetherer : DBConection
    {
        public const int STEP = 5;
        protected int offset;
        protected int limit;
        protected int file_offset;
        protected bool loadFromFile;
        protected string loadFromPath;
        protected IniFiles settings;


        public KwGetherer()
        {
            this.init();
        }

        private void init()
        {
            this.clearCSV();
          
            this.offset = 0;
            this.limit = STEP;
            this.file_offset = 0;
            this.loadFromFile = false;
            this.loadFromPath = "";

            settings = new IniFiles("Settings.ini");


            this.limit = !settings.KeyExists("limit") ?
               Int32.Parse(settings.Write("limit", "" + STEP)) :
               Int32.Parse(settings.Read("limit"));

            this.loadFromFile = !settings.KeyExists("loadFromFile") ?
              Boolean.Parse(settings.Write("loadFromFile", "false")) :
              Boolean.Parse(settings.Read("loadFromFile"));

            this.loadFromPath = !settings.KeyExists("loadFromPath") ?
                settings.Write("loadFromPath", "words.txt") :
                settings.Read("loadFromPath");

            if (loadFromFile)
                this.offset = !settings.KeyExists("file_offset") ?
                    Int32.Parse(settings.Write("file_offset", "" + file_offset)) :
                    Int32.Parse(settings.Read("file_offset"));
            else
                this.offset = !settings.KeyExists("offset") ?
                   Int32.Parse(settings.Write("offset", "" + offset)) :
                   Int32.Parse(settings.Read("offset"));
        }


        private void clearCSV()
        {
            Directory.GetFiles(Directory.GetCurrentDirectory()).
                ToList().
                ForEach(file =>
                {
                    if (file.IndexOf(".csv") != -1)
                        File.Delete(file);
                });
        }

        public async void execute()
        {
            while (true)
            {
                List<DBKeyword> list = loadFromFile ? loadFile(offset, limit) : this.listKeywords(offset, limit);
                int count = list.Count();                

                List<Task> taskList = new List<Task>();
                list.ForEach(kw =>
                {                    
                   var task =  Task.Run( () =>   this.takeKW(kw) );
                   taskList.Add(task);
                });
                Console.WriteLine("Задач в таске:" + taskList.Count());
                Task.WaitAll(taskList.ToArray());

                this.offset += STEP;
                settings.Write(this.loadFromFile ? "file_offset" : "offset", "" + this.offset);

                if (loadFromFile && offset >= count)
                {
                    settings.Write("loadFromFile", "false");
                    offset = Int32.Parse(settings.Read("offset"));
                    loadFromFile = false;
                    count = this.countKewyrods();
                    Console.WriteLine("Переключаемся на выборку из БД");
                }

                if (offset >= count)
                    break;
            }

        }

        private async void takeKW(DBKeyword kw)
        {

            ChromeDriver driver = this.initDriver(kw);
            if (!driver.isSelectorExist(By.CssSelector(".report-download-button")))
            {
                driver.Close();
                return;
            }

            IWebElement element = driver.FindElement(By.CssSelector(".report-download-button"));
            string fileName = element.GetAttribute("download");
            Console.WriteLine("Скачиваем:" + fileName);
            element.SendKeys(Keys.Enter);

            while (!File.Exists(fileName))
                System.Threading.Thread.Sleep(2000);
            driver.Close();

            File.ReadAllLines(fileName)
                .Skip(1)
                .ToList()
                .ForEach(s =>
                {
                    string[] buf = s.Split(';');
                    string keyword = (new Regex("['\"]")).Replace(buf[0], "");

                    if (int.Parse(buf[1]) <= 7 && int.Parse(buf[4]) <= 100)
                    {
                        if (this.isKeywordExist(keyword) == -1)
                            this.Insert(keyword);
                    }

                });
            File.Delete(fileName);

        }


        public List<DBKeyword> loadFile(int offset, int limit)
        {
            if (!File.Exists(this.loadFromPath))
                throw new FileNotFoundException(this.loadFromPath);

            List<DBKeyword> list = new List<DBKeyword>();
            File.ReadAllLines(this.loadFromPath)
                .Skip(offset)
                .Take(limit)
                .ToList()
                .ForEach(s =>
                {
                    DBKeyword dbw = new DBKeyword();
                    dbw.keyword = s;
                    list.Add(dbw);
                });

            return list;
        }

        private ChromeDriver initDriver(DBKeyword kw)
        {
            var options = new ChromeOptions();
            //options.AddArgument("no-sandbox");
            options.AddUserProfilePreference("download.default_directory", Directory.GetCurrentDirectory());
            options.AddArguments("--disable-extensions");
            // options.AddArgument("no-sandbox");
            options.AddArgument("--incognito");
            //options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");  //--d

            ChromeDriver driver = new ChromeDriver(options);//открываем сам браузер
            driver.LocationContext.PhysicalLocation = new OpenQA.Selenium.Html5.Location(55.751244, 37.618423, 152);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //время ожидания компонента страницы после загрузки страницы
            driver.Manage().Cookies.DeleteAllCookies();

            driver.Navigate().GoToUrl("http://www.bukvarix.com/keywords/?q=" + kw.keyword + "&r=report");

            return driver;
        }

    }
}
