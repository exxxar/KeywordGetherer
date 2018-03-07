using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class TestClassTable:DBConection
    {
        public string test;
        public string test2;
        public DateTime updated_at;

        public TestClassTable()
        {
            this.test = "ERRRR";
            this.test2 = "FFFFFF";
            this.updated_at = DateTime.Now;
    }
    }
}
