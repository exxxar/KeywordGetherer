using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    public class Forecastinfo
    {
        public int Clicks { get; set; }
        public int FirstPlaceClicks { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public int PremiumClicks { get; set; }
        public decimal PremiumMin { get; set; }
        public decimal PremiumMax { get; set; }
        public int Shows { get; set; }
        public int CTR { get; set; }
        public string Currency { get; set; }
        public bool is_preceded { get; set; }
        public decimal ContextPrice { get; set; }
        public decimal FirstPlaceCtr { get; set; }
        public decimal PremiumCtr { get; set; }
        public int Keyword_id { get; set; }

        

    }
}
