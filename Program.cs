using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace KeywordGetherer
{
    class Program
    {
//        azyexxxar;906fe2dd055a4f6fa08b0765156e6cf7;AQAAAAAhzCtcAASm9aZlZ6gwzUielkMitN3PEsc
//azyexxxar;06254bf26eea43758fba5d9145da951e;AQAAAAAhzCtcAATSXRQ8IJtLjElcrfX8_XU_JDw
//vipglamur-ru;06254bf26eea43758fba5d9145da951e;AQAAAAAMIkwEAATSXYtsBxWf8UwltY901Itp7zs
//koleca.ru;06254bf26eea43758fba5d9145da951e;AQAAAAALtx_HAATSXSGsB75L80KUmb_WvkpgseE
//TRB-BM-odnk;06254bf26eea43758fba5d9145da951e;AQAAAAANcG9vAATSXRIMU7_E4Erupp8EnRwTtG8
//unidentdirect;06254bf26eea43758fba5d9145da951e;AQAAAAAUAE0qAATSXUyoJ4pvhUbXl9pswZpzMKY

        static void Main(string[] args)
        {

            //Task.Run(() => (new YandexEtherGetherer()).execute());
            // Task.Run(() => (new ForecastGetherer()).execute());
            //// Task.Run(() => (new KeywordGetherer()).execute());
            //Console.ReadLine();
            //return;
          
            foreach (string s in args)
            {
                switch (s.Trim().ToLower())
                {
                    case "forecast": Task.Run(() => (new ForecastGetherer()).execute()); break;
                    default:
                    case "keywords": Task.Run(() => (new KeywordGetherer()).execute()); break;
                    case "ether":    Task.Run(() => (new YandexEtherGetherer()).execute()); break;

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
