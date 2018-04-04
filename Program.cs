using KeywordGetherer;
using KeywordGetherer.Markov;
using KeywordGetherer.SiteParser;
using MarkVSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace KeywordGetherer
{
    class Program
    {
        public static string arguments = "";

        static void Main(string[] args)
        {
            arguments = String.Join(" ", args);

            Type t = typeof(SystemController);
            MethodInfo[] attrs = t.GetMethods();
            bool findeMethod = false;
            foreach (MethodInfo m in attrs)
            {
                foreach (CustomAttributeData cd in m.CustomAttributes)
                {

                    ExecutebleAttribute tx = (ExecutebleAttribute)Attribute.GetCustomAttribute(m, typeof(ExecutebleAttribute));
                    MatchCollection mc = null;

                    if (tx != null)
                    {
                        Regex reg = new Regex(tx.command);

                        mc = reg.Matches(args.Length > 0 ? args[0] : "usage");

                    }

                    if (cd.AttributeType == typeof(ExecutebleAttribute)
                        && mc.Count != 0
                      )
                    {
                        try
                        {

                            List<object> obj = new List<object>();
                            string[] buf = new string[args.Length > 0 ? args.Length-1  : 0];
                            if (args.Length > 1)
                            {
                                Array.Copy(args, 1, buf, 0, args.Length-1 );
                                obj.AddRange(buf);
                            }
                            findeMethod = true;
                            m.Invoke(Activator.CreateInstance(typeof(SystemController)), buf.Length > 0 ? obj.ToArray() : null);

                        }
                        catch (TargetParameterCountException e)
                        {
                            Console.WriteLine("недостаточно параметров! Должно быть {0}!", m.GetParameters().Length);
                        }
                        catch (TargetInvocationException e)
                        {
                            Console.WriteLine("Exception: {0}", e);

                        }
                    }
                }
            }


            if (!findeMethod)
                Console.WriteLine("Метод не найден! Попробуйте usage");

            ThreadPool.SetMaxThreads(10000, 1000);

            Task.Run(() => (new SelfRestarter(TimeSpan.FromHours(25))).execute());
            Console.ReadLine();
        }
    }
}
