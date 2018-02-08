using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yandex.Direct.Configuration;
using Yandex.Direct.Authentication;
using Yandex.Direct;

namespace KeywordGetherer
{
    class YandexDirect:DBConection
    {
        private const int MAX_FORECAST = 50;

        private YandexDirectConfiguration _ydc;
        private YandexDirectService _yds;

        public YandexDirect(string login, string appId, string token, YandexApiLanguage lang = YandexApiLanguage.English)
        {
            try
            {
                _ydc = new YandexDirectConfiguration();
                _ydc.AuthProvider = new TokenAuthProvider(login, appId, token);
                _ydc.Language = lang;
                _ydc.ServiceUrl = new Uri("https://soap.direct.yandex.ru/json-api/v4/");
                _yds = new YandexDirectService(_ydc);
                Console.WriteLine("Подключились к Forecast");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
       
        private void createForecastReport(string[] keywords, int[] regions)
        {
            _yds.CreateNewForecast(keywords, regions);
        }

        private void awaitForReportReady()
        {
            bool ready = false;
            while (!ready)
            {
                ready = true;
                _yds.GetForecastList().ForEach(fs =>
                {                   
                    Console.WriteLine(fs.ForecastId + " " + fs.Status);
                    System.Threading.Thread.Sleep(1500);
                    ready &= fs.Status.Equals(ReportStatus.Done);
                });
               
            }
            Console.WriteLine("Все отчеты готовы!");


        }

        private void getReport(int reportId)
        {

            ForecastInfo fi = _yds.GetForecast(reportId);
            
          
            fi.Phrases.ToList().ForEach(_fbpi=>
            {
                Forecastinfo fc = new Forecastinfo();
                fc.Clicks = _fbpi.Clicks;
                fc.ContextPrice = _fbpi.ContextPrice;
                fc.FirstPlaceClicks = _fbpi.FirstPlaceClicks;
                fc.FirstPlaceCtr = _fbpi.FirstPlaceCtr;
                fc.PremiumClicks = _fbpi.PremiumClicks;
                fc.PremiumCtr = _fbpi.PremiumCtr;
                fc.PremiumMax = _fbpi.PremiumMax;
                fc.PremiumMin = _fbpi.PremiumMin;
                fc.CTR = 0;
                fc.Shows = _fbpi.Shows;
                fc.is_preceded = _fbpi.Phrase.IndexOf("!") != -1 ? true : false;
                fc.Keyword_id = this.isKeywordExist(_fbpi.Phrase.restoringPrecede()) ? this.getKeywordId(_fbpi.Phrase.restoringPrecede()) : -1;
                this.InsertForecast(fc);
                

                Console.WriteLine(_fbpi.Phrase + " " + _fbpi.Clicks + " " + _fbpi.Shows);
            });           
            removeReport(reportId);

        }

        public async void execute(string [] keywords)
        {
            try
            {
                int[] regions = new int[] { 1 };
                
                createForecastReport(keywords, regions);
                awaitForReportReady();
                _yds.GetForecastList().ForEach(fs=>getReport(fs.ForecastId));            

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void removeReport(int reportId)
        {
            Console.WriteLine("Удаляем отчет:" + reportId);
            _yds.DeleteForecastReport(reportId);
        }

        private void removeAllReports()
        {
            _yds.GetForecastList().ForEach(fs => removeReport(fs.ForecastId)); 
        }
    }
}
