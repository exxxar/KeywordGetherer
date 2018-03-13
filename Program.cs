using KeywordGetherer;
using KeywordGetherer.Markov;
using KeywordGetherer.SiteParser;
using MarkVSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

                    case "file":
                        string path = "";
                        try
                        {
                            theradsCount = int.TryParse(args[i + 1], out theradsCount) ? theradsCount : THREAD_COUNT;
                            Console.WriteLine(theradsCount);
                            path = args[i + 2];
                            Console.WriteLine(path);
                        }
                        catch { }

                        List<Task> taskList2 = new List<Task>();
                        while (true)
                        {
                            taskList2.Add(Task.Run(() => (new SiteAndKeywords(path)).execute()));

                            if (taskList2.Count >= theradsCount)
                            {
                                Task.WaitAll(taskList2.ToArray());
                                taskList2.Clear();
                            }
                            Thread.Sleep(500);
                        }
                        break;

                    case "crawler":

                        try
                        {
                            theradsCount = int.TryParse(args[i + 1], out theradsCount) ? theradsCount : THREAD_COUNT;
                            Console.WriteLine(theradsCount);
                           
                        }
                        catch { }

                        List<Task> taskList3 = new List<Task>();
                        while (true)
                        {
                            taskList3.Add(Task.Run(() => (new UrlCrawler()).execute()));

                            if (taskList3.Count >= theradsCount)
                            {
                                Task.WaitAll(taskList3.ToArray());
                                taskList3.Clear();
                            }
                            Thread.Sleep(500);
                        }
                        break;
                        break;


                    case "markov":
                        int size = 1;
                        string file_path = "";
                        string type = "p";
                        try
                        {
                            file_path = args[i + 1];
                            type = args[i + 2];
                            size = int.TryParse(args[i + 3], out size) ? size : 1;                            
                            Console.WriteLine(size);
                            if (!File.Exists(file_path))
                            {
                                Console.WriteLine("файле то где?!");
                                return;
                            }

                        }
                        catch { }

                        string buf = "";
                        switch (type)
                        {
                            default:
                            case "p": buf = (new MarkovGen(file_path)).paragrpaph(size); break;
                            case "s": buf = (new MarkovGen(file_path)).sentence(size); break;
                            case "w": buf = (new MarkovGen(file_path)).words(size); break;
                            case "t": buf = (new MarkovGen(file_path)).title(size); break;
                        }
                        Console.WriteLine(buf);
                        break;
                    case "?":
                    case "help":
                    case "usage":
                        Console.WriteLine("commands: forecast, keywords, ether, url, ya_engine, file, crawler, markov"); break;

                    case "? -markov":
                    case "help -markov":
                    case "usage -markov":
                        Console.WriteLine("p - paragrap, w - word, s - sentance, t - title"); break;

                }
            }
            Task.Run(() => (new SelfRestarter(TimeSpan.FromHours(5), arguments)).execute());
            Console.ReadLine();
        }
    }
}
