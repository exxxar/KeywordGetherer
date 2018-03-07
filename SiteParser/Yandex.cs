using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer.SiteParser
{
    class Yandex:DBConection
    {
        public String url = "http://yandex.ru";
        public ChromeDriver driver;
        public long regionId = 1;

        public Yandex()
        {      
            var options = new ChromeOptions();
            options.AddArguments("--disable-extensions");
            options.AddArgument("no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-gpu-shader-disk-cache");
            options.AddArgument("--headless");

            driver = new ChromeDriver(options);//открываем сам браузер

            driver.LocationContext.PhysicalLocation = new OpenQA.Selenium.Html5.Location(55.751244, 37.618423, 152);

            driver.Manage().Window.Maximize();//открываем браузер на полный экран
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //время ожидания компонента страницы после загрузки страницы
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Navigate().GoToUrl(this.url);
        }

        public void open_settings()
        {
            Console.WriteLine("ENTER BLOCK: настройка региона поиска https://yandex.ru/tune/geo/?retpath=https%3A%2F%2Fwww.yandex.ru%2F%3Fdomredir%3D1&nosync=1");
            IWebElement element = null;
            Actions actions = null;

            driver.Navigate().GoToUrl("https://yandex.ru/tune/geo/?retpath=https%3A%2F%2Fwww.yandex.ru%2F%3Fdomredir%3D1&nosync=1");
            element = driver.FindElement(By.Id("city__front-input"));
            actions = new Actions(driver);
            actions.MoveToElement(element).Click().Perform();
            element.Clear();
            element.SendKeys("Москва");
            System.Threading.Thread.Sleep(YandexUtils.rndSleep());
            element.SendKeys(Keys.Down);
            System.Threading.Thread.Sleep(YandexUtils.rndSleep());
            element.SendKeys(Keys.Enter);
            System.Threading.Thread.Sleep(YandexUtils.rndSleep());
            //element = driver.FindElement(By.CssSelector(".form__save"));
            //actions.MoveToElement(element).Click().Perform();
            Console.WriteLine("EXIT BLOCK: настройка региона поиска https://yandex.ru/tune/geo/?retpath=https%3A%2F%2Fwww.yandex.ru%2F%3Fdomredir%3D1&nosync=1");

        }

        public void change_search_pos_count()
        {
            Console.WriteLine("ENTER BLOCK: установка величины поисковой выдачи https://yandex.ru/search/customize");
            driver.Navigate().GoToUrl("https://yandex.ru/search/customize");

            IWebElement element = null;
            Actions actions = null;

            element = driver.FindElement(By.CssSelector(".fieldset:nth-of-type(1) .field__content .radio:nth-of-type(2)"));
            actions = new Actions(driver);
            actions.MoveToElement(element).Click().Perform();
            System.Threading.Thread.Sleep(YandexUtils.rndSleep());
            element = driver.FindElement(By.CssSelector(".fieldset:nth-of-type(2) .field__content .radio:nth-of-type(4)"));
            actions = new Actions(driver);
            actions.MoveToElement(element).Click().Perform();
            System.Threading.Thread.Sleep(YandexUtils.rndSleep());
            element = driver.FindElement(By.CssSelector(".form__submit button[name='save']"));
            actions = new Actions(driver);
            actions.MoveToElement(element).Click().Perform();
            System.Threading.Thread.Sleep(YandexUtils.rndSleep());
            Console.WriteLine("EXIT BLOCK: установка величины поисковой выдачи https://yandex.ru/search/customize");
        }
        public bool isValidRegion(String region = "Москва")
        {
            //.geolink__reg             
            IWebElement element = driver.FindElement(By.CssSelector(".geolink__reg"));
            return element.Text.ToLower().Equals(region.ToLower());
        }

        public void search(DBKeyword keyword)
        {
            
            driver.Navigate().GoToUrl(this.url + "/search/?text=" + keyword.keyword);

            int adTopCount = 0,
                adBottomCount = 0,
                searchCount = 0;

            Boolean flagTB = true, isAd = true;

            foreach (IWebElement i in driver.FindElements(By.CssSelector(".serp-item")))
            {

                if (i.GetAttribute("class").IndexOf("serp-adv-item") == -1)
                {
                    flagTB = false;
                    isAd = false;
                    ++searchCount;
                }

                if (i.GetAttribute("class").IndexOf("serp-adv-item") != -1 && flagTB)
                {
                    ++adTopCount;
                    isAd = true;
                }
                else
                    if (i.GetAttribute("class").IndexOf("serp-adv-item") != -1 && !flagTB)
                {
                    ++adBottomCount;
                    isAd = true;
                }
                String localUrl = "";
                String description = "";

                try
                {
                    localUrl = i.FindElement(By.CssSelector(".link_outer_yes b")).Text;
                    description = i.FindElement(By.CssSelector(".organic__title-wrapper")).Text;
                }
                catch (Exception e) { }

                if (localUrl.Trim() == "" && description.Trim() == "")
                    continue;

                Console.WriteLine("l_url=>{0}, d=>{1}", localUrl, description);

                long urlId = isUrlExist(localUrl);
                if (urlId == -1)
                    urlId = Insert_Site(localUrl);

                Keyword kw = new Keyword();
                //kw.url = url;
                kw.keyword_id = keyword.keyword_id;
                kw.description = description;
                kw.is_ad = isAd;
                kw.region_id = this.regionId;
                kw.site_id = urlId;
                if (isAd)
                    kw.position = (byte)(flagTB ? adTopCount : 4 + adBottomCount);
                else
                    kw.position = (byte)searchCount;
                kw.search_engine = (byte)Keyword.SearchEngine.YANDEX;
                kw.created_at = DateTime.Now;
                kw.updated_at = DateTime.Now;

                
                try
                {
                    if (!isExist_AdSearchPosition(kw))
                        Insert_AdSearchPosition(kw);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

        }

        public void exit()
        {
            //закрываем драйвер и закрываем браузер
            driver.Close();
            driver.Quit();
        }

        public bool isSelectorExist(By selector)
        {
            return driver.FindElements(selector).Count != 0;
        }



    }
}
