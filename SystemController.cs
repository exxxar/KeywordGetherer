﻿using KeywordGetherer.Markov;
using KeywordGetherer.SiteParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    public class SystemController
    {
        [Help("Запуск сборщика цифровой инфы по словам. Без параметров.")]
        [Executeble("forecast")]
        public void executeForecast()
        {
            Task.Run(() => (new ForecastGetherer()).execute());
        }

        [Help("Запуск сборщика слов.")]
        [Executeble("keywords")]
        public void executeKwGether()
        {
            Task.Run(() => (new KeywordGetherer()).execute());
        }

        [Help("Запуск выборки поисковых позиций.")]
        [Executeble("ya_engine")]
        public void executeYaEngine(object theradsCount)
        {
            List<Task> taskList = new List<Task>();
            while (true)
            {
                taskList.Add(Task.Run(() => (new YandexGetherer()).execute()));

                if (taskList.Count >= int.Parse((string)theradsCount))
                {
                    Task.WaitAll(taskList.ToArray());
                    taskList.Clear();
                }
            }
        }

        [Help("Запуск выборки ключевых слов по доменам.")]
        [Executeble("url")]
        public void executeKwBySite(object theradsCount)
        {
            Task.Run(() => (new KeywordsBySiteGetherer()).execute());
        }

        [Help("Запуск яндекс.эфира")]
        [Executeble("ether")]
        public void executeEther(object theradsCount)
        {
            Task.Run(() =>
            {
                (new YandexEtherGetherer((int)theradsCount)).execute();
            });
        }

        [Help("Выгрузка данных из файла.")]
        [Executeble("file")]
        public void executeFromFile(object theradsCount, object path)
        {
            List<Task> taskList = new List<Task>();
            while (true)
            {
                taskList.Add(Task.Run(() => (new SiteAndKeywords((string)path)).execute()));

                if (taskList.Count >= int.Parse((string)theradsCount))
                {
                    Task.WaitAll(taskList.ToArray());
                    taskList.Clear();
                }
                Thread.Sleep(500);
            }
        }


        [Help("Запуск сборщика текстов по сайтам из базы ")]
        [Executeble("crawler")]
        public void executeCrawler(object theradsCount)
        {
            List<Task> taskList = new List<Task>();
            while (true)
            {
                taskList.Add(Task.Run(() => (new UrlCrawler()).execute()));

                if (taskList.Count >= int.Parse((string)theradsCount))
                {
                    Task.WaitAll(taskList.ToArray());
                    taskList.Clear();
                }
                Thread.Sleep(500);
            }
        }

        [Help("Запуск генерации слов цепями Маркова 3 параметра: тип, размерность, путь к файлу-словарю")]
        [Executeble("markov")]
        public void executeMarkov(object type, object size, string path)
        {
            Console.WriteLine("type={0} size={1} path={2}", type, size, path);
            string buf = "";
            switch ((String)type)
            {
                default:
                case "p": buf = (new MarkovGen(path)).paragrpaph(int.Parse((string)size)); break;
                case "s": buf = (new MarkovGen(path)).sentence(int.Parse((string)size)); break;
                case "w": buf = (new MarkovGen(path)).words(int.Parse((string)size)); break;
                case "t": buf = (new MarkovGen(path)).title(int.Parse((string)size)); break;
            }
            Console.WriteLine(buf);
        }


        [Executeble("usage")]
        public void executeUsage()
        {
            Console.WriteLine("Usage:");

            Type t = this.GetType();
            MethodInfo[] attrs = t.GetMethods();
            StringBuilder buf = new StringBuilder();
            foreach (MethodInfo m in attrs)
            {
                foreach (CustomAttributeData cd in m.CustomAttributes)
                {

                    HelpAttribute ha = (HelpAttribute)Attribute.GetCustomAttribute(m, typeof(HelpAttribute));
                    ExecutebleAttribute ea = (ExecutebleAttribute)Attribute.GetCustomAttribute(m, typeof(ExecutebleAttribute));

                    if (ha != null && ea != null
                        && buf.ToString().IndexOf(ea.command) == -1)
                    {
                        StringBuilder param = new StringBuilder();
                        buf.Append(ea.command);
                        for (int i = 0; i < m.GetParameters().Length; i++)
                            param.AppendFormat(i != m.GetParameters().Length - 1 ? "{0}," : "{0}", m.GetParameters()[i].Name);

                        Console.WriteLine("Методы [{0}]=>{1}", ea.command, ha.usage);
                        Console.WriteLine("Параметров=>{0} {1} \n", m.GetParameters().Length, param.ToString());
                    }
                }
            }

        }
    }
}
