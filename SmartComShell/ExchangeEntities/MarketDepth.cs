using System;

namespace SmartComShell
{
    public class MarketDepth
    {
        public MarketDepth(int depth)
        {
            this.depth = depth;
            Asks = new Quote[depth];
            Bids = new Quote[depth];
        }

        public DateTime Time { set; get; }

        public Security Security { set; get; }

        private int depth;
        public int Depth 
        { 
            get { return depth; }
        }

        public Quote[] Asks { private set; get; }
        public Quote[] Bids { private set; get; }


        private Quote[] _asks;
        private Quote[] _bids;


        internal bool IsUpdate { set; get; }

        internal void Update(int row, int nrows, double bid, double bidsize, double ask, double asksize)
        { 
            if (row == 0)
            {
                _asks = new Quote[depth];
                _bids = new Quote[depth];
            }

            _asks[row] = new Quote((decimal)ask, (decimal)asksize, eSide.Sell);
            _bids[row] = new Quote((decimal)bid, (decimal)bidsize, eSide.Buy);

            if (row == Depth -1 || row == nrows -1)
            { 
                Object lck = new Object();

                lock(lck)
                {
                    Asks = _asks;
                    Bids = _bids;
                }

                Time = DateTime.Now;
                _asks = null;
                _bids = null;

                IsUpdate = true;

            }
        
        }


    }
}
