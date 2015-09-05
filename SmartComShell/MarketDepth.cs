using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public class MarketDepth
    {
       
        public MarketDepth()
        {
             Asks = new List<Quote>();
             Bids = new List<Quote>();
        }

        public DateTime Time { set; get; }

        public int Depth { set; get; }

        public List<Quote> Asks { private set; get; }
        public List<Quote> Bids { private set; get; }

        internal void Update()
        { }

    }
}
