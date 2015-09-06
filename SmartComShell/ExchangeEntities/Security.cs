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

        private MarketDepth depth;
        public MarketDepth Depth 
        { 
            get 
            {
                if (depth == null) Depth = new MarketDepth(20); 
                return depth; 
            } 
            set { depth = value;} 
        }

        public Trade LastTrade { set; get; }

        private List<Trade> trades;
        public List<Trade> Trades
        { 
            get 
            {
                if (trades == null) Trades = new List<Trade>();
                return trades; 
            }
            set { trades = value; }
        }

    }
}
