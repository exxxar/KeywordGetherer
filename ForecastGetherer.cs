using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class ForecastGetherer:DBConection
    {
        private const int LIMIT = 50;
        protected int forecastOffset;
        protected IniFiles settings;

        public ForecastGetherer()
        {
            settings = new IniFiles("Settings.ini");

            this.forecastOffset = !settings.KeyExists("forecast_offset") ?
               Int32.Parse(settings.Write("forecast_offset", "0")) :
               Int32.Parse(settings.Read("forecast_offset"));
        }
        public async void execute()
        {
            File.ReadAllLines("accounts.txt")
                .ToList()
                .ForEach(s=>
                {
                    //azyexxxar 906fe2dd055a4f6fa08b0765156e6cf7 AQAAAAAhzCtcAASm9aZlZ6gwzUielkMitN3PEsc
                    string[] buf = s.Split(';');
                    Console.WriteLine("Выбираем аккаунт Login [{0}], AppId [{1}], Token [{2}]", buf[0], buf[1], buf[2]);
                    Task.Run(() =>
                    {
                        YandexDirect yandexDirect = new YandexDirect(buf[0], buf[1], buf[2]);
                        while (true)
                        {
                            List<string> temp = new List<string>();
                            List<DBKeyword> tempList = this.listKeywords(this.forecastOffset, LIMIT);
                            if (tempList.Count == 0 || tempList == null)
                                break;

                            tempList
                            .ForEach(w =>
                            {
                                Console.WriteLine("Формируем Forecast для [{0}]", w.keyword);
                                Console.WriteLine("Формируем Forecast для [{0}]", w.keyword.checkLenAndSlice().divideAndPrecede());
                                temp.Add(w.keyword);
                                temp.Add(w.keyword.checkLenAndSlice().divideAndPrecede());

                            });

                            yandexDirect.execute(temp.ToArray());

                            this.forecastOffset += LIMIT;
                            settings.Write("forecast_offset", "" + this.forecastOffset);
                        }

                    });
                });
        }

   
    }
}
