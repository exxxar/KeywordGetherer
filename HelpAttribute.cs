using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class HelpAttribute:System.Attribute
    {
        public string usage { get; set; }

        public HelpAttribute(string usage = "")
        {
            this.usage = usage;
        }
    }
}
