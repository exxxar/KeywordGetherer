using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace KeywordGetherer.SiteParser
{
    class SiteAndKeywords : DBConection
    {
        const string SETTINGS_SECTION = "site_and_keyword_file_parser";

        private static Mutex kwMutext = new Mutex();
        protected int STEP;
        protected long offset;
        protected int limit;
        protected IniFiles settings;
        protected string path;



        public SiteAndKeywords(string path, int step = 1000)
        {
            this.path = path;
            this.STEP = step;
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

            this.offset = !settings.KeyExists("offset", SETTINGS_SECTION) ?
                  long.Parse(settings.Write("offset", "" + offset, SETTINGS_SECTION)) :
                  long.Parse(settings.Read("offset", SETTINGS_SECTION));
        }

        public async void execute()
        {
            try
            {
                string oldUrl = "";
                while (true)
                {
                    kwMutext.WaitOne();
                    var globalOffset = !settings.KeyExists("offset", SETTINGS_SECTION) ?
                        long.Parse(settings.Write("offset", "" + offset, SETTINGS_SECTION)) :
                        long.Parse(settings.Read("offset", SETTINGS_SECTION));

                    this.offset = Math.Max(this.offset, globalOffset);

                    this.offset += this.limit;
                  
                   
                    this.loadFile(this.offset, this.limit)
                        .ToList()
                        .ForEach(item =>
                    {


                        string[] buf = item.Split(';');
                        string url = buf[0].Replace("\"", "");
                        string keyword = buf[1].Replace("\"", "");
                      //  Console.WriteLine("url=>[{0}] keyword=>[{1}]", url, keyword);

                       
                        if (!url.Equals(oldUrl))
                        {
                            oldUrl = url;
                            if (this.isUrlExist(url) == -1)
                                this.Insert_Site(url);
                        }

                        if (keyword.IndexOf("?") == -1)
                            if (!this.isKeywordExist(keyword))
                                this.Insert(keyword);
                       
                    });
                    settings.Write("offset", "" + this.offset, SETTINGS_SECTION);
                    kwMutext.ReleaseMutex();
                }
            }
            catch (Exception e)
            {
                Task.Run(() => (new SelfRestarter(TimeSpan.FromSeconds(25))).execute());
            }

        }

        public IEnumerable<string> loadFile(long offset, int limit)
        {
            if (!File.Exists(this.path))
                throw new FileNotFoundException(this.path);

            List<string> list = new List<string>();

            File.ReadLines(this.path)
                .Skip(Math.Max(1, (int)offset))
                .Take(limit)
                .ToList()
                .ForEach(s => list.Add(s));

            return list;
        }
    }
}
