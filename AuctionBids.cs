using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordGetherer
{
    public class AuctionBids
    {
        public string Position { get; set; }
        public float Bid { get; set; }
        public float Price { get; set; }
        public long forecastInfo_id { get; set; }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf
                .AppendFormat("Pos={0} ",Position)
                .AppendFormat(" Bid={0} ",Bid)
                .AppendFormat(" Price={0} ",Price)
                .AppendFormat(" F_id={0} ",forecastInfo_id);
            return buf.ToString();
        }
    }
}
