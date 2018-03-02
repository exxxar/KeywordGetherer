using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace KeywordGetherer
{
    class Program
    {
        const int THRAD_COUNT = 5;

        static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(10000, 1000);
           
            

            string arguments = "";
            for (int i = 0; i < args.Length; i++)
            {
                string s = args[i];
                arguments += s;
                switch (s.Trim().ToLower())
                {
                    case "forecast":
                        Task.Run(() =>
                        {
                            (new ForecastGetherer()).execute();
                            
                        }); break;
                    case "keywords":
                        Task.Run(() =>
                        {
                     
                            (new KeywordGetherer()).execute();
                          
                        }); break;
                    case "url":
                        Task.Run(() =>
                        {
                        
                            (new KeywordsBySiteGetherer()).execute();
                          
                        }); break;
                    case "ether":
                        int theradsCount = 1;
                        try
                        {
                            theradsCount = int.TryParse(args[i + 1], out theradsCount) ? theradsCount : THRAD_COUNT;
                        }
                        catch { }
                        Console.WriteLine("ALL THREADS=" + theradsCount);
                        Task.Run(() =>
                        {
                         
                            (new YandexEtherGetherer(theradsCount)).execute();
                           
                        });
                        break;

                    case "?":
                    case "help":
                    case "usage":
                        Console.WriteLine("commands: forecast, keywords, ether, url"); break;

                }
            }
            Task.Run(() => (new SelfRestarter(TimeSpan.FromHours(5), arguments)).execute());
            Thread.Sleep(15000);
            //if (Program.allThreadCount == 0)
            //{
            //    Task.Run(() =>
            //    {
            //        (new ForecastGetherer()).execute();
            //        Program.allThreadCount++;
            //    });
            //}
            Console.ReadLine();
        }
    }
}
