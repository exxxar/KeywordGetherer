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
    class YandexDirect : DBConection
    {
        private const int MAX_FORECAST = 50;

        private YandexDirectConfiguration _ydc;
        private YandexDirectService _yds;

        public YandexDirect(string login, string appId, string token, YandexApiLanguage lang = YandexApiLanguage.English)
        {
            try
            {
                this._ydc = new YandexDirectConfiguration();
                this._ydc.AuthProvider = new TokenAuthProvider(login, appId, token);
                this._ydc.Language = lang;
                this._ydc.ServiceUrl = new Uri("https://api.direct.yandex.ru/live/v4/json/");
                this._yds = new YandexDirectService(_ydc);
                Console.WriteLine("Подключились к Forecast");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void createForecastReport(string[] keywords, int[] regions)
        {
            try
            {
                this._yds.CreateNewForecast(keywords, regions);
            }
            catch (Exception e)
            {
                Console.WriteLine("МЫ ТУТ УПАЛИ!" + e.Message);
            }
        }

        private void awaitForReportReady()
        {
            bool ready = false;
            while (!ready)
            {
                ready = true;
                this._yds.GetForecastList().ForEach(fs =>
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
            Console.WriteLine("Получаем отчет с айди {0}", reportId);

            fi.Phrases.ToList().ForEach(_fbpi =>
            {

                Console.WriteLine("Глянем что в AuctionBids, а там {0}", _fbpi.AuctionBids==null?"пусто":"что-то есть");
                Forecastinfo fc = new Forecastinfo();
                fc.Clicks = _fbpi.Clicks;
                fc.ContextPrice = _fbpi.ContextPrice;
                fc.FirstPlaceClicks = _fbpi.FirstPlaceClicks;
                fc.FirstPlaceCtr = _fbpi.FirstPlaceCtr;
                fc.PremiumClicks = _fbpi.PremiumClicks;
                fc.PremiumCtr = _fbpi.PremiumCtr;
                fc.PremiumMax = _fbpi.PremiumMax;
                fc.PremiumMin = _fbpi.PremiumMin;
                fc.CTR = fc.CTR;
                fc.Shows = _fbpi.Shows;
                fc.is_preceded = _fbpi.Phrase.IndexOf("!") != -1 ? true : false;
                fc.Keyword_id = this.isKeywordExist(_fbpi.Phrase.restoringPrecede()) ? this.getKeywordId(_fbpi.Phrase.restoringPrecede()) : -1;
                int addedId = this.InsertForecast(fc);
                Console.WriteLine("Добавлен ForecastInfo c id=> "+addedId);
                if (addedId != -1 && _fbpi.AuctionBids != null)
                {
                    _fbpi
                        .AuctionBids
                        .ToList()
                        .ForEach(_ab_item =>
                        {

                            AuctionBids ab = new AuctionBids();
                            ab.Position = _ab_item.Position;
                            ab.Bid = _ab_item.Bid;
                            ab.Price = _ab_item.Price;
                            ab.forecastInfo_id = addedId;
                            this.InsertAuctionBids(ab);
                        });
                    Console.WriteLine("Добавление AuctionBids для фразы=> " + _fbpi.Phrase);
                }
             
            });
            removeReport(reportId);

        }

        public async void execute(string[] keywords)
        {
            try
            {
                int[] regions = new int[] { 1 };
                Console.WriteLine("Создаем отчет");
                createForecastReport(keywords, regions);
                Console.WriteLine("Ждем пока подготовится отчет");
                awaitForReportReady();
                Console.WriteLine("Разбираем инфу из отчета");
                _yds.GetForecastList().ForEach(fs => getReport(fs.ForecastId));

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
