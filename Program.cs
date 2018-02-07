using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Yandex.Direct.Configuration;
using Yandex.Direct.Authentication;
using Yandex.Direct;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class Program
    {
        static void Main(string[] args)
        {

            KeywordGetherer kwGetherer = new KeywordGetherer();
           // ForecastGetherer fcGetherer = new ForecastGetherer();

            Task.Run(() => kwGetherer.execute());
            Console.ReadLine();
            //Task.Run(() => fcGetherer.execute());
        }
    }
}
