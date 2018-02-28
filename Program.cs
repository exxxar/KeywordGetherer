using System;
using System.Threading.Tasks;


namespace KeywordGetherer
{
    class Program
    {
        const int THRAD_COUNT = 5;
        static void Main(string[] args)
        {

            for (int i=0;i<args.Length;i++)
            {
                string s = args[i];
                switch (s.Trim().ToLower())
                {
                    case "forecast": Task.Run(() => (new ForecastGetherer()).execute()); break;
                    case "keywords": Task.Run(() => (new KeywordGetherer()).execute()); break;
                    case "ether":
                        int theradsCount = int.TryParse(args[i + 1], out theradsCount)? theradsCount : THRAD_COUNT;
                       
                        Console.WriteLine("ALL THERADS="+theradsCount);
                        Task.Run(() => (new YandexEtherGetherer(theradsCount)).execute());
                        break;

                    case "?":
                    case "help":
                    case "usage":
                        Console.WriteLine("commands: forecast, keywords, ether"); break;                      

                }
            }
            Console.ReadLine();
        }
    }
}
