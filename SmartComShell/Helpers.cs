using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartComShell
{
    public static class Helpers
    {

        public static Security CreateSecurity(string symbol, string short_name, string long_name, string type,
                                                int decimals, int lot_size, double punkt, double step, string sec_ext_id,
                                                string sec_exch_name, System.DateTime expiry_date, double days_before_expiry, double strike)
        {

            eSecurityType? sectype = SmartToSecurityType(type);

            if (sectype != null)
            {
                return new Security()
                {
                    ID = "",
                    Code = "",
                    Board = SmartToBoard(sec_exch_name).Value,
                    Type = sectype.Value,
                    Symbol = symbol,
                    ShortName = short_name,
                    LongName = long_name,
                    TickSize = (decimal)step,
                    TickCost = (decimal)punkt,
                    Decimals = decimals,
                    LotSize = lot_size,
                    Expiration = sectype == eSecurityType.Future || sectype == eSecurityType.Option ? (DateTime?)expiry_date : null,
                    Strike = sectype == eSecurityType.Option ? (decimal?)strike : null
                };
            }

            return null;

        }

        public static eSecurityType? SmartToSecurityType(string type)
        {
            switch (type)
            {
                case "Future":
                    {
                        return eSecurityType.Future;
                    }
                case "Stock":
                    {
                        return eSecurityType.Stock;
                    }
                case "Option":
                    {
                        return eSecurityType.Option;
                    }
                case "Index":
                    {
                        return eSecurityType.Index;
                    }
                case "Bond":
                    {
                        return eSecurityType.Bond;
                    }
                case "Curency":
                    {
                        return eSecurityType.Curency;
                    }
                default:
                    break;
            }

            return null;

        }

        public static eBoard? SmartToBoard(string type)
        {
            switch (type)
            {
                case "FORTS":
                    {
                        return eBoard.Forts;
                    }
                case "Stock":
                    {
                        return eBoard.Micex;
                    }
                default:
                    break;
            }

            return null;

        }


        public static eSide? SmartToSide(SmartCOM3Lib.StOrder_Action action)
        {
            switch (action)
            {
                case SmartCOM3Lib.StOrder_Action.StOrder_Action_Buy :
                    {
                        return eSide.Buy;
                    }
                case SmartCOM3Lib.StOrder_Action.StOrder_Action_Sell :
                    {
                        return eSide.Sell;
                    }
                default:
                    break;
            }

            return null;

        }

        public static Security Lookup(this Connector connector, string id, string field = "" )
        {
            Security security = null;

            if (string.IsNullOrWhiteSpace(field) || field.ToUpper() == "ID")
            {
                security = connector.Securities.FirstOrDefault(s => s.ID == id);
            }
            else if (field.ToUpper() == "CODE")
            {
                security = connector.Securities.FirstOrDefault(s => s.Code == id);
            }
            else if (field.ToUpper() == "SYMBOL")
            {
                security = connector.Securities.FirstOrDefault(s => s.Symbol == id);
            }
             return security;
        }

        public static Trade CreateTrade(Security security, System.DateTime datetime, double price, double volume, string tradeno, SmartCOM3Lib.StOrder_Action action)
        {
            return new Trade()
            {
                ID = tradeno,
                Time = datetime,
                Price = (decimal)price,
                Volume = (decimal)volume,
                Direction = SmartToSide(action).Value,
                Security = security
            };
        }





        //Symbol string Код ЦБ из таблицы котировок SmartTrade Short_name string Краткое наименование Long_name string Полное наименование Type string Код типа из справочника SmartTrade Decimals int Точность цены Lot_size int Размер лота ценных бумаг punkt double Цена минимального шага step double Минимальный шаг цены sec_ext_id string ISIN sec_exch_name string Наименование площадки expiryDate DateTime Дата экспирации days_before_expiry double Дней до экспирации strike double Цена исполнения) 

        //Row int Номер сделки в списке NRows int Всего сделок в списке 
        //    



    }
}
