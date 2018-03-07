using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static KeywordGetherer.DBConection;

namespace KeywordGetherer.SiteParser
{
    class YandexGetherer : DBConection
    {
        const string SETTINGS_SECTION = "yandex_engine_keywords_searcher";

        private static Mutex kwMutext = new Mutex();
        protected int STEP;
        protected long offset;
        protected int limit;
        protected int threadCount;
        protected IniFiles settings;



        public YandexGetherer(int step = 5, int threadCount = 1)
        {
            this.STEP = step;
            this.threadCount = threadCount;
            this.init();
        }

        private void init()
        {
            this.offset = 0;
            this.limit = STEP;

            settings = new IniFiles(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Settings.ini");

            this.limit = !settings.KeyExists("limit", SETTINGS_SECTION) ?
               Int32.Parse(settings.Write("limit", "" + STEP, SETTINGS_SECTION)) :
               Int32.Parse(settings.Read("limit", SETTINGS_SECTION));

        }


        public async void execute()
        {

            Yandex yandex = new Yandex();

            if (!yandex.isValidRegion())
                yandex.open_settings();
            yandex.change_search_pos_count();

            try
            {
                while (true)
                {
                    kwMutext.WaitOne();

                    this.offset = !settings.KeyExists("offset", SETTINGS_SECTION) ?
                        long.Parse(settings.Write("offset", "" + offset, SETTINGS_SECTION)) :
                        long.Parse(settings.Read("offset", SETTINGS_SECTION));

                    this.limit = STEP;
                    this.offset += this.limit;
                    settings.Write("offset", "" + this.offset, SETTINGS_SECTION);

                    List<DBKeyword> list = this.listKeywords(offset, limit);
                    if (list == null)
                    {
                        kwMutext.ReleaseMutex();
                        break;
                    }

                    kwMutext.ReleaseMutex();

                    foreach (DBKeyword kw in list)
                    {
                        yandex.search(kw);
                        Thread.Sleep(YandexUtils.rndSleep());
                    }

                }
            }catch { }
            yandex.exit();
        }
    }
}
