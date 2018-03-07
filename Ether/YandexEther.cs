using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace KeywordGetherer
{
    public class YandexEther : DBConection
    {
        private static Mutex connectMutext = new Mutex();
        private string url;

        public YandexEther(string etherUrl)
        {
            this.url = etherUrl;
        }
        public async void parse(Boolean useSlicer = true)
        {
            StreamReader objReader;

            try
            {
                objReader = new StreamReader(this.connect());
            }
            catch
            {
                return;
            }

            string sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    Regex regex = new Regex(@">([a-zA-Zа-яА-Я0-9 ]+)</");

                    MatchCollection matches = regex.Matches(sLine);
                    if (matches.Count > 0)
                    {
                        string keyword = matches[0]
                            .Value
                            .Replace(">", "")
                            .Replace("</", "")
                            .Trim()
                            .checkLenAndSlice();

                        if (keyword.Length <= 3)
                            continue;

                        if (!this.isKeywordExist(keyword))
                            this.Insert(keyword);
                        if (useSlicer)
                            keyword.sliceAndTakeVariants();
                    }
                }
            }
            objReader.Close();
            objReader.Dispose();
        }

        private Stream connect()
        {
            connectMutext.WaitOne();
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(this.url);

            WebProxy myProxy = new WebProxy("myproxy", 80);
            myProxy.BypassProxyOnLocal = true;

            wrGETURL.Proxy = WebProxy.GetDefaultProxy();

            Stream objStream;

            objStream = wrGETURL.GetResponse().GetResponseStream();

            connectMutext.ReleaseMutex();
            return objStream;
        }
    }
}
