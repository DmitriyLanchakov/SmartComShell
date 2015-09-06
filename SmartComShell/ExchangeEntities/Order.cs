using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public class Order
    {

        public Portfolio Portfolio { set; get; }
        public Security Security { set; get; }
        public eSide Direction { set; get; }
        public eOrderType Type { set; get; }
        public eDuration Duration { set; get; }
        public eOrderState State { set; get; }
        public decimal Price { set; get; }
        public decimal StopPrice { set; get; }
        public decimal Balance { set; get; }
        public decimal Volume { set; get; }
        public int UserID { set; get; }


        //  void PlaceOrder(string portfolio, 
        //  string symbol, 
        //  StClientLib.StOrder_Action action, 
        // StClientLib.StOrder_Type type, 
        // StClientLib.StOrder_Validity validity, 
        // double price, double amount, double stop, int cookie)
        //Наименование Тип


    }
}
