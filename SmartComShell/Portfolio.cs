using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public class Portfolio
    {
        public Portfolio(string name, string boarddName)
        {
            Name = name;
            Board = eBoard.Forts;
        }


        public string Name { private set; get; }

        public eBoard Board { private set; get; }

        public decimal Balance { private set; get; }

        public decimal Leverage { private set; get; }

        public decimal Saldo { private set; get; }

        public decimal Comission { private set; get; }

        public DateTime Time { private set; get; }

        internal void Update(double cash, double leverage, double comission, double saldo)
        {
            Time = DateTime.Now;

            if (Balance != (decimal)cash)
            { 
                Balance = (decimal)cash; 
            }

            if (Leverage != (decimal)leverage)
            {
                Leverage = (decimal)leverage; 
            }

            if (Comission != (decimal)comission)
            {
                Comission = (decimal)comission; 
            }

            if (Saldo != (decimal)saldo)
            { 
                Saldo = (decimal)saldo; 
            }
        }



        

    }
}
