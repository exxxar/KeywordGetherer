using KeywordGetherer;
using KeywordGetherer.SiteParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace KeywordGetherer
{
    class Program
    {
        const int THREAD_COUNT = 5;

        static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(10000, 1000);


            //TestClassTable test = new TestClassTable();

            //Console.WriteLine(test.insert());
            //Console.ReadLine();

            //return;
            int theradsCount = 1;
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
                    case "ya_engine":
                        try
                        {
                            theradsCount = int.TryParse(args[i + 1], out theradsCount) ? theradsCount : THREAD_COUNT;
                        }
                        catch { }

                        List<Task> taskList = new List<Task>();
                        while (true)
                        {
                            taskList.Add(Task.Run(() => (new YandexGetherer()).execute()));

                            if (taskList.Count >= theradsCount)
                            {
                                Task.WaitAll(taskList.ToArray());
                                taskList.Clear();
                            }
                        }
                        break;
                    case "url":
                        Task.Run(() =>
                        {

                            (new KeywordsBySiteGetherer()).execute();

                        }); break;
                    case "ether":                      
                        try
                        {
                            theradsCount = int.TryParse(args[i + 1], out theradsCount) ? theradsCount : THREAD_COUNT;
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
                        Console.WriteLine("commands: forecast, keywords, ether, url, ya_engine"); break;

                }
            }
            Task.Run(() => (new SelfRestarter(TimeSpan.FromHours(5), arguments)).execute());
            Console.ReadLine();
        }
    }
}
