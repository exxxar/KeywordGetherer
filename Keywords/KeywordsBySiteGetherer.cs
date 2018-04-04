using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class KeywordsBySiteGetherer : DBConection
    {
        private static Mutex kwsiteMutext = new Mutex();
        protected int STEP;
        protected long offsetSite;
        protected int limitSite;
        protected IniFiles settings;


        public KeywordsBySiteGetherer(int step=5)
        {
            this.STEP = step;
            this.init();
        }

        private void init()
        {
            this.clearCSV();

            this.offsetSite = 0;
            this.limitSite = STEP;


            settings = new IniFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Settings.ini");


            this.offsetSite = !settings.KeyExists("offsetSite") ?
                   Int32.Parse(settings.Write("offsetSite", "0")) :
                   Int32.Parse(settings.Read("offsetSite"));

            this.limitSite = !settings.KeyExists("limitSite") ?
                    Int32.Parse(settings.Write("limitSite", "" + STEP)) :
                    Int32.Parse(settings.Read("limitSite"));

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
            Console.WriteLine("Запуск процесса зборщика слова по URL");
            List<Task> taskList = new List<Task>();

            while (true)
            {
                kwsiteMutext.WaitOne();
               var globalOffset = !settings.KeyExists("offsetSite") ?
                    Int32.Parse(settings.Write("offsetSite", "0")) :
                    Int32.Parse(settings.Read("offsetSite"));

                this.offsetSite = Math.Max(this.offsetSite, globalOffset);

                this.limitSite = (STEP - Math.Min(taskList.Count, STEP));
                this.offsetSite += (STEP - Math.Min(taskList.Count, STEP));
                globalOffset += this.limitSite;



                settings.Write("offsetSite", "" + this.offsetSite);

                List<String> list = this.listSites(offsetSite, limitSite);
                if (list == null)
                {
                    kwsiteMutext.ReleaseMutex();
                    break;
                }
                kwsiteMutext.ReleaseMutex();

                list.ForEach(site =>
                {
                    taskList.Add(Task.Run(() =>
                    {
                        this.takeKwBySite(site);
                     
                    }));
                    Thread.Sleep(YandexUtils.rndSleep());
                });
                Console.WriteLine("Задач в таске:" + taskList.Count());


                if (taskList.Count >= this.limitSite)
                {
                    Task.WaitAny(taskList.ToArray());
                    try
                    {
                        taskList
                            .Where(t => t.IsCompleted)
                            .ToList()
                            .ForEach(t =>
                            {
                                taskList.Remove(t);

                            });
                    }
                    catch { }
                }
                Thread.Sleep(YandexUtils.rndSleep());
            }

        }

        private async void takeKwBySite(string siteUrl, Boolean useSlicer = true)
        {

            ChromeDriver driver = this.initDriver(siteUrl);
            if (!driver.isSelectorExist(By.CssSelector(".report-download-button")))
            {
                driver.Close();
                return;
            }

            IWebElement element = driver.FindElement(By.CssSelector(".report-download-button"));
            
            string fileName = "msk_"+siteUrl+".csv";
            Console.WriteLine("Скачиваем:" + fileName);
            element.SendKeys(Keys.Enter);

            while (!File.Exists(fileName))
                Thread.Sleep(YandexUtils.rndSleep());
            driver.Close();
            try
            {
                File.ReadAllLines(fileName)
                    .Skip(1)
                    .ToList()
                    .ForEach(s =>
                    {
                        string[] buf = s.Split(';');
                        string keyword = (new Regex("['\"]")).Replace(buf[0], "");

                        if (!this.isKeywordExist(keyword.checkLenAndSlice()))
                            this.Insert(keyword.checkLenAndSlice());

                        if (useSlicer)
                            keyword.sliceAndTakeVariants();

                    });
            }
            catch (Exception e)
            {
                Console.WriteLine("Мы тут упали:" + e.Message);
                Task.Run(() => (new SelfRestarter(TimeSpan.FromSeconds(25))).execute());
            }
            File.Delete(fileName);

        }


        private ChromeDriver initDriver(string siteUrl)
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

            driver.Navigate().GoToUrl("http://www.bukvarix.com/mcmp/?q3=" + siteUrl + "&region=msk&v=table");

            return driver;
        }
    }
}
