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
using Yandex.Direct.Domain.Forecasts;

namespace KeywordGetherer
{
    class Program
    {
        static void test()
        {
            YandexDirectConfiguration _ydc;
            YandexDirectService _yds;

            _ydc = new YandexDirectConfiguration();
            _ydc.AuthProvider = new TokenAuthProvider("azyexxxar", "906fe2dd055a4f6fa08b0765156e6cf7", "AQAAAAAhzCtcAASm9aZlZ6gwzUielkMitN3PEsc");
            _ydc.Language = YandexApiLanguage.English;
            _ydc.ServiceUrl = new Uri("https://api.direct.yandex.ru/live/v4/json/");
            _yds = new YandexDirectService(_ydc);
            Console.WriteLine("Подключились к Forecast");
            //  _yds.GetForecastList().ForEach(fs => _yds.DeleteForecastReport(fs.ForecastId));
            _yds.CreateNewForecast(new string[] { "двери" }, new int[] { 1 });

            bool ready = false;
            while (!ready)
            {
                ready = true;
                _yds.GetForecastList().ForEach(fs =>
                {
                    Console.WriteLine(fs.ForecastId + " " + fs.Status);
                    System.Threading.Thread.Sleep(1500);
                    ready &= fs.Status.Equals(ReportStatus.Done);

                    if (fs.Status.Equals(ReportStatus.Done))
                    {
                        ForecastInfo fi = _yds.GetForecast(fs.ForecastId);
                        Console.WriteLine("Получаем отчет с айди {0}", fs.ForecastId);

                        foreach (ForecastBannerPhraseInfo fbi in fi.Phrases)
                        {
                            try
                            {
                                Console.WriteLine("test=>" + fbi.Currency);
                                foreach(PhraseAuctionBids pab in fbi.AuctionBids)
                                {
                                    Console.WriteLine("test-auc=>" + pab.Position);
                                }
                                

                            }
                            catch { }
                        }
                    }
                });

            }
            Console.WriteLine("Все отчеты готовы!");
        }


        static void Main(string[] args)
        {
           // test();
            //return;
            foreach (string s in args)
            {
                switch (s.Trim().ToLower())
                {
                    case "forecast": Task.Run(() => (new ForecastGetherer()).execute()); break;
                    default:
                    case "keywords": Task.Run(() => (new KeywordGetherer()).execute()); break;
                }
            }

            Console.ReadLine();


            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.StackTrace);
            //}
            Console.ReadLine();
        }
    }
}
