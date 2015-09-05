using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public class Trade
    {
        public DateTime Time { set; get; }
        public string ID { set; get; }
        public decimal Price { set; get; }
        public decimal Volume { set; get; }
        public eSide Direction { set; get; }
        public Security Security { set; get; }
    }
}
