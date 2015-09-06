using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    [Flags]
    public enum eBoard
    {
        Forts = 1,
        Micex =2
    }

    [Flags]
    public enum eSecurityType
    {
        Stock = 1,
        Future = 2,
        Option = 4,
        Curency = 8,
        Bond = 16,
        Index = 32
    }

    public enum eSide
    {
        Sell,
        Buy
    }

    public enum eOrderType
    {
        Limit,
        Market,
        Stop,
        StopLimit
    }

    public enum eOrderState
    {
        Limit,
        Stop,
        Market
    }

    public enum eDuration
    {
        Day,
        GTC
    }



    public enum eSmartServer
    {
        Matrix,
        Demo,
        Reserve1,
        Reserve2
    }

    public enum eChange
    {
        New,
        Change,
        Remove
    }


}
