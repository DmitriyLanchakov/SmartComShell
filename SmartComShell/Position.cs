using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public class Position
    {
        public DateTime Time { private set; get; }
        public Security Security { private set; get; }
        public Portfolio Portfolio { private set; get; }
        public decimal AveragePrice { private set; get; }
        public decimal Volume { private set; get; }
        public decimal Planned { private set; get; }

        internal void Update (double avprice, double amount, double planned)
        {
            Time = DateTime.Now;
            AveragePrice = (decimal)avprice;
            Volume = (decimal)amount;
            Planned = (decimal)planned;
        }
       

    }
}
