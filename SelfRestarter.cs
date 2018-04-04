using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    public class SelfRestarter:DBConection
    {
        private long selfRestartTime;
        private string argument;

        public SelfRestarter(TimeSpan restartIntervalTime)
        {        
            this.selfRestartTime = DateTime.Now.TimeOfDay.Ticks+restartIntervalTime.Ticks;
            this.argument = Program.arguments;
        }

        public async void execute()
        {
            Dictionary<string, long> counts_delta= null;
            long deltaTime = DateTime.Now.TimeOfDay.Ticks + TimeSpan.FromMinutes(5).Ticks;
            try
            {
                counts_delta = getTablesCountInfo();
            } catch {
            }
            while (true)
            {
                Console.WriteLine("Ping {0} / {1}. We are live!",  DateTime.Now.TimeOfDay, TimeSpan.FromTicks(selfRestartTime));
                if (selfRestartTime <= DateTime.Now.TimeOfDay.Ticks)
                {                   
                   Process.Start(Assembly.GetExecutingAssembly().Location, this.argument);                    
                    Process.GetCurrentProcess().Kill();
                }

                try
                {
                    if (counts_delta["forecastinfo"] == getTablesCountInfo()["forecastinfo"]
                        && counts_delta["keywords"] == getTablesCountInfo()["keywords"]
                        && deltaTime <= DateTime.Now.TimeOfDay.Ticks
                        )
                    {
                        Console.WriteLine("Мы давно наблюдали обновления БД! Возможно у нас что-то пошло не так!");
                        Process.Start(Assembly.GetExecutingAssembly().Location, this.argument);
                        Process.GetCurrentProcess().Kill();
                    }
                }
                catch { }

                try
                {
                    if (counts_delta["forecastinfo"] != getTablesCountInfo()["forecastinfo"]
                       || counts_delta["keywords"] != getTablesCountInfo()["keywords"])
                    {
                        deltaTime = DateTime.Now.TimeOfDay.Ticks + TimeSpan.FromMinutes(5).Ticks;
                        counts_delta = getTablesCountInfo();
                    }
                }catch { }

                int worker = 0;
                int io = 0;
                ThreadPool.GetAvailableThreads(out worker, out io);

                //Console.WriteLine("Thread pool threads available at startup: ");
                //Console.WriteLine("   Worker threads: {0:N0}", worker);
                //Console.WriteLine("   Asynchronous I/O threads: {0:N0}", io);
                //Console.WriteLine("   Count threads: {0}", Process.GetCurrentProcess().Threads.Count);

                Thread.Sleep(1000);
                                
            }
        }
    }
}
