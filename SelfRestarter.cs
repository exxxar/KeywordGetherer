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
                Console.WriteLine("Ping {0} / {1}. We are live!",DateTime.Now.TimeOfDay, TimeSpan.FromTicks(selfRestartTime));
                if (selfRestartTime <= DateTime.Now.TimeOfDay.Ticks)
                {                   
                   Process.Start(Assembly.GetExecutingAssembly().Location, this.argument);
                    Process.GetCurrentProcess().CloseMainWindow();
                    Process.GetCurrentProcess().Kill();
                }
                Thread.Sleep(1000);
            }
        }
    }
}
