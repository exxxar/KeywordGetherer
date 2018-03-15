using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    [AttributeUsage(AttributeTargets.Method)]
    class ExecutebleAttribute:System.Attribute
    {
        public string command { get; set; }

        public ExecutebleAttribute()
        {
            this.command = "";
        }

        public ExecutebleAttribute(string command = "")
        {
            this.command = command;
        }

    }
}
