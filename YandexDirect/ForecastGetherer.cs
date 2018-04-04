using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class ForecastGetherer : DBConection
    {
        const string SETTINGS_SECTION = "forecast_getherer";

        private static Mutex forecastMutext = new Mutex();
        private const int LIMIT = 50;
        protected Int64 forecastOffset;
        protected IniFiles settings;


        public ForecastGetherer()
        {
            settings = new IniFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Settings.ini");

            this.forecastOffset = !settings.KeyExists("offset", SETTINGS_SECTION) ?
               Int64.Parse(settings.Write("offset", "0", SETTINGS_SECTION)) :
               Int64.Parse(settings.Read("offset", SETTINGS_SECTION));
        }
        public async void execute()
        {
            File.ReadAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/accounts.txt")
                .ToList()
                .ForEach(s =>
                {

                    string[] buf = s.Split(';');
                    Console.WriteLine("Выбираем аккаунт Login [{0}], AppId [{1}], Token [{2}]", buf[0], buf[1], buf[2]);
                    Task.Run(() =>
                    {

                        YandexDirect yandexDirect = new YandexDirect(buf[0], buf[1], buf[2]);
                        while (true)
                        {
                            forecastMutext.WaitOne();
                            var globalOffset = !settings.KeyExists("offset", SETTINGS_SECTION) ?
                                        Int64.Parse(settings.Write("offset", "0", SETTINGS_SECTION)) :
                                        Int64.Parse(settings.Read("offset", SETTINGS_SECTION));

                            this.forecastOffset = Math.Max(this.forecastOffset, globalOffset);
                            Console.WriteLine("Take date from {0} pos to {1}", this.forecastOffset, this.forecastOffset + LIMIT);


                            List<string> temp = new List<string>();
                            List<DBKeyword> tempList = this.listKeywords(this.forecastOffset, LIMIT);
                            if (tempList.Count == 0 || tempList == null)
                                break;

                            this.forecastOffset += LIMIT;
                            globalOffset += LIMIT;

                            settings.Write("offset", "" + Math.Max(this.forecastOffset,globalOffset), SETTINGS_SECTION);
                            forecastMutext.ReleaseMutex();

                            tempList
                            .ForEach(w =>
                            {
                                temp.Add(w.keyword);
                                temp.Add(w.keyword.checkLenAndSlice().divideAndPrecede());

                            });
                            Console.WriteLine("Запускаем отчет");
                            yandexDirect.execute(temp.ToArray());


                        }

                    });
                });
        }


    }
}
