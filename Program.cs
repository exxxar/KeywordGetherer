using System;
using System.Threading.Tasks;


namespace KeywordGetherer
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => (new ForecastGetherer()).execute());
            Task.Run(() => (new KeywordGetherer()).execute());
            Console.ReadLine();
            return;
            foreach (string s in args)
            {
                switch (s.Trim().ToLower())
                {
                    case "forecast": Task.Run(() => (new ForecastGetherer()).execute()); break;
                    default:
                    case "keywords": Task.Run(() => (new KeywordGetherer()).execute()); break;
                }
            }


            Console.ReadLine();
        }
    }
}
