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
    public class SelfRestarter
    {
        private long selfRestartTime;
        private string argument;
        public SelfRestarter(TimeSpan restartIntervalTime,String argument)
        {        
            this.selfRestartTime = DateTime.Now.TimeOfDay.Ticks+restartIntervalTime.Ticks;
            this.argument = argument;       
        }

        public async void execute()
        {
            while (true)
            {
                Console.WriteLine("Ping {0} / {1}. We are live!",  DateTime.Now.TimeOfDay, TimeSpan.FromTicks(selfRestartTime));
                if (selfRestartTime <= DateTime.Now.TimeOfDay.Ticks)
                {                   
                   Process.Start(Assembly.GetExecutingAssembly().Location, this.argument);
                    Process.GetCurrentProcess().CloseMainWindow();
                    Process.GetCurrentProcess().Kill();
                }

                int worker = 0;
                int io = 0;
                ThreadPool.GetAvailableThreads(out worker, out io);

                Console.WriteLine("Thread pool threads available at startup: ");
                Console.WriteLine("   Worker threads: {0:N0}", worker);
                Console.WriteLine("   Asynchronous I/O threads: {0:N0}", io);
                Console.WriteLine("   Count threads: {0}", Process.GetCurrentProcess().Threads.Count);

                Thread.Sleep(1000);
                                
            }
        }
    }
}
