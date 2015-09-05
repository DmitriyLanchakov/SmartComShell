using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public class Security
    {
        public string ID {  set; get; }
        public string Code { set; get; }
        public eBoard Board { set; get; }
        public eSecurityType Type { set; get; }
        public decimal TickSize { set; get; }
        public decimal TickCost { set; get; }
        public int Decimals { set; get; }
        public int LotSize { set; get; }
        public DateTime? Expiration { set; get; }
        public decimal? Strike { set; get; }

        internal string Symbol { set; get; }
        internal string ShortName { set; get; }
        internal string LongName { set; get; }

        public MarketDepth Depth { set; get; }
        public Trade LastTrade { set; get; }


    }
}
