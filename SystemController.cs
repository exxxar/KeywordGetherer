using KeywordGetherer.Markov;
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

                if (taskList.Count >= (long)theradsCount)
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

                if (taskList.Count >= (long)theradsCount)
                {
                    Task.WaitAll(taskList.ToArray());
                    taskList.Clear();
                }
                Thread.Sleep(500);
            }
        }


        [Help("Запуск сборщика текстов по сайтам из базы, 1 параметр: кол-во потокв ")]
        [Executeble("crawler")]
        public void executeCrawler(object theradsCount)
        {
            List<Task> taskList = new List<Task>();
            while (true)
            {
                taskList.Add(Task.Run(() => (new UrlCrawler()).execute()));

                if (taskList.Count >= (long)theradsCount)
                {
                    Task.WaitAll(taskList.ToArray());
                    taskList.Clear();
                }
                Thread.Sleep(500);
            }
        }

        [Help("Запуск генерации слов цепями Маркова 3 параметра: тип, размерность, путь к файлу-словарю")]
        [Executeble("markov")]
        public void executeMarkov(object type, object size, object path)
        {
            string buf = "";
            switch ((String)type)
            {
                default:
                case "p": buf = (new MarkovGen((string)path)).paragrpaph((int)size); break;
                case "s": buf = (new MarkovGen((string)path)).sentence((int)size); break;
                case "w": buf = (new MarkovGen((string)path)).words((int)size); break;
                case "t": buf = (new MarkovGen((string)path)).title((int)size); break;
            }
            Console.WriteLine(buf);
        }


        [Executeble("usage")]
        public void executeUsage()
        {
            Console.WriteLine("usage");

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
                        && buf.ToString().IndexOf(String.Format("Method [{0}]=>{1} \n", ea.command, ha.usage)) == -1)
                        buf.AppendFormat("Method [{0}]=>{1} \n", ea.command, ha.usage);


                }
            }
            Console.WriteLine(buf.ToString());
        }
    }
}
