using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    public class YandexEtherGetherer
    {
        public int taskLimit { get; set; }
        public string etherUrl { get; set; }
        public YandexEtherGetherer(string etherUrl= "https://export.yandex.ru/last/last20x.xml", int taskLimit = 5)
        {
            this.taskLimit = taskLimit;
            this.etherUrl = etherUrl;
        }
        public async void execute()
        {
            List<Task> taskList = new List<Task>();
            while (true)
            {
                var task = Task.Run(() => (new YandexEther(this.etherUrl)).parse());
                taskList.Add(task);
                Console.WriteLine("=====>Добавлена задача!Всего {0} задач", taskList.Count);
                Thread.Sleep(3000);
                if (taskList.Count == this.taskLimit)
                {                   
                    Task.WaitAny(taskList.ToArray());
                    try
                    {                       
                        taskList
                            .Where(t => t.IsCompleted)
                            .ToList()
                            .ForEach(t => taskList.Remove(t));
                    }
                    catch {  }
                }

            }
        }
    }
}
