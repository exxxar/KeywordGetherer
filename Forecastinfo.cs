using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    class Forecastinfo:DBConection
    {
        public int Clicks { get; set; }
        public int FirstPlaceClicks { get; set; }
        public string Geo { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public int PremiumClicks { get; set; }
        public decimal PremiumMin { get; set; }
        public int Shows { get; set; }

        public decimal ContextPrice { get; set; }
        public decimal FirstPlaceCtr { get; set; }
        public decimal PremiumCtr { get; set; }

    }
}
