using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    public class YandexEtherGetherer
    {
        public int taskLimit { get; set; }
        public string etherUrl { get; set; }
        public YandexEtherGetherer(int taskLimit = 5, string etherUrl = "https://export.yandex.ru/last/last20x.xml")
        {
            this.taskLimit = taskLimit;
            this.etherUrl = etherUrl;

        }
        public async void execute()
        {
            if (this.taskLimit <= 0)
                return;

            List<Task> taskList = new List<Task>();
            while (true)
            {
                try
                {
                    taskList.Add(Task.Run(() =>
                    {
                        (new YandexEther(this.etherUrl)).parse();
                   }));
                }
                catch {
                    Task.Run(() => (new SelfRestarter(TimeSpan.FromSeconds(25))).execute());
                }

                Console.WriteLine("=====>Добавлена задача!Всего {0} задач", taskList.Count);
                Thread.Sleep(YandexUtils.rndSleep());
                if (taskList.Count >= this.taskLimit)
                {
                    Task.WaitAny(taskList.ToArray());
                    try
                    {
                        taskList
                            .Where(t => t.IsCompleted)
                            .ToList()
                            .ForEach(t =>
                            {
                                taskList.Remove(t);
                               
                            });
                    }
                    catch {
                        Task.Run(() => (new SelfRestarter(TimeSpan.FromSeconds(25))).execute());
                    }
                }

            }
        }
    }
}
