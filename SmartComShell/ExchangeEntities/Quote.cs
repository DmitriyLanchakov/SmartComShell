using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public struct Quote
    {
        public Quote(decimal price, decimal volume, eSide direction)
        { 
            this.price = price;
            this.volume =volume;
            this.direction = direction;
        }

        private decimal price;
        public decimal Price 
        { 
            get { return price;}
            private set { price = value; }
        }

        private decimal volume;
        public decimal Volume 
        { 
            get { return volume; }
            private set { volume = value; }
        }

        private eSide direction;
        public eSide Direction 
        { 
            get { return direction; }
            private set { direction = value; }
        }


    }
}
