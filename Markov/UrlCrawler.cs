
using MarkVSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeywordGetherer.Markov
{
    class UrlCrawler
    {
        public string SiteUrl = @"http://greenfinance.ru";
        List<String> urlsInSite = new List<string>();
        public UrlCrawler()
        {
            urlsInSite.Add(SiteUrl);

            int index = 0;
            if (!File.Exists("data.txt"))
                File.Create("data.txt");

            while(index<urlsInSite.Count)
            {

                string localUrl = urlsInSite.ToArray()[index];
                Console.WriteLine("Берем [{0}] url", localUrl);
                ///System.Threading.Thread.Sleep(5000);
                WebRequest webr = null;
                HttpWebResponse resp = null;

                

                try
                {
                    webr = WebRequest.Create(localUrl);
                    resp = (HttpWebResponse)webr.GetResponse();
                }
                catch(NotSupportedException nse)
                {
                    index++;
                    continue;
                }
                catch(UriFormatException ue)
                {
                    index++;
                    continue;
                }
                catch (WebException e)
                {
                    resp = (HttpWebResponse)e.Response;
                  
                }

                if (resp == null)
                {
                    
                    index++;
                    continue;
                }
                Stream stream = resp.GetResponseStream();
                
                StreamReader sr = new StreamReader(stream, Encoding.GetEncoding(resp.CharacterSet));


                string sReadData = sr.ReadToEnd()
                    .Replace("\n", String.Empty)
                    .Replace("\t", String.Empty);

              


                //   Console.WriteLine(sReadData);

                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK:        //HTTP 200 - всё ОК
                                                   //

                        Regex reg = new Regex("<a href=\"([^\"]*)\">");
                        MatchCollection mc = reg.Matches(sReadData);
                        foreach (Match m in mc)
                        {
                            string rez = m.Groups[1].Value.ToLower();

                            Regex reg_img = new Regex("(jpg|png|gif|bmp|pdf|jpeg|doc|docx)");

                            //
                            if (!Array.Exists(urlsInSite.ToArray(), element => element == (rez.IndexOf("http") == -1 ? SiteUrl + rez : rez)) && reg_img.Matches(rez).Count == 0)
                            {

                                urlsInSite.Add(rez.IndexOf("http") == -1 ? SiteUrl + rez : rez);
                                Console.WriteLine(rez.IndexOf("http") == -1 ? SiteUrl + rez : rez);
                            }
                        }

                        reg = new Regex(@"<p[^>]*>\s*(.*?)\s*<\/p>");
                        mc = reg.Matches(sReadData);
                        StringBuilder stb = new StringBuilder();
                        foreach (Match m in mc)
                        {
                            string rez = Regex
                                .Replace(m.Groups[1].Value, "<[^>]+>", string.Empty);
                           
                            stb.Append(WebUtility.HtmlDecode(rez));
                        }


                        using (StreamWriter sw = File.AppendText("data.txt"))
                        {
                            
                            sw.WriteLine(stb.ToString());
                            sw.Flush();
                            sw.Close();
                        }
                        break;

                            
                    case HttpStatusCode.Forbidden: //HTTP 403 - доступ запрещён
                        break;
                    case HttpStatusCode.NotFound:  //HTTP 404 - документ не найден
                        break;
                    case HttpStatusCode.Moved:     //HTTP 301 - документ перемещён
                        break;
                    default:                       //другие ошибки
                        break;
                }
                index++;
            }

            GeneratorFacade gen = new GeneratorFacade(new MarkovGenerator(System.IO.File.ReadAllText("data.txt")));

            Console.WriteLine(gen.GenerateParagraphs(4));
        }
    }
}
