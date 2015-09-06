using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

using SmartCOM3Lib;



namespace SmartComShell
{
    public class Connector
    {
        private SmartCOM3Lib.StServer _smartServer;

        private static Connector _connector;

        private Object _lock = new Object();

        private int _lastUserId;

        private Connector()
        {
            _smartServer = new StServer();

            _smartServer.Connected += _smartServer_Connected;
            _smartServer.Disconnected += _smartServer_Disconnected;
            _smartServer.AddPortfolio += _smartServer_AddPortfolio;
            _smartServer.AddSymbol += _smartServer_AddSymbol;
            _smartServer.SetPortfolio += _smartServer_SetPortfolio;
            _smartServer.UpdatePosition += _smartServer_UpdatePosition;
            _smartServer.AddTick += _smartServer_AddTick;
            _smartServer.UpdateBidAsk += _smartServer_UpdateBidAsk;
            _smartServer.UpdateQuote +=  _smartServer_UpdateQuote;
            _smartServer.AddTrade += _smartServer_AddTrade;
            _smartServer.UpdateOrder += _smartServer_UpdateOrder;
            _smartServer.SetMyClosePos += _smartServer_SetMyClosePos;
            _smartServer.SetMyOrder += _smartServer_SetMyOrder;
            _smartServer.SetMyTrade += _smartServer_SetMyTrade;
            _smartServer.OrderSucceeded += _smartServer_OrderSucceeded;
            _smartServer.OrderFailed += _smartServer_OrderFailed;
            _smartServer.OrderCancelFailed += _smartServer_OrderCancelFailed;
            _smartServer.OrderCancelSucceeded += _smartServer_OrderCancelSucceeded;
            _smartServer.OrderMoveFailed += _smartServer_OrderMoveFailed;
            _smartServer.OrderMoveSucceeded += _smartServer_OrderMoveSucceeded;
            
            //_smartServer.AddBar += new _IStClient_AddBarEventHandler(_smartServer_AddBar);
            //_smartServer.AddTickHistory += new _IStClient_AddTickHistoryEventHandler(_smartServer_AddTickHistory);

            Portfolios = new List<Portfolio>();
            Positions = new List<Position>();
            ClosedPositions = new List<Position>();
            Securities = new List<Security>();
            RegisteredSecurities = new List<Security>();
            RegisteredTrades = new List<Security>();
            RegisteredMarketDepths = new List<Security>();
            Orders = new List<Order>();
            UserTrades = new List<UserTrade>();
        }

        public Connector GetInstance()
        {
            if (_connector == null)
            {
                 lock(_lock)
                 {
                     _connector = new Connector();
                 }
            }
            return _connector;
        }

        #region События Connector

        public event Action Connected = delegate { };

        private void OnConnected()
        {
            if (Connected != null) Connected();
        }

        public event Action<string> Disconnected = delegate { };

        private void OnDisconnected(string reason)
        {
            if (Disconnected != null) Disconnected(reason);
        }

        public event Action<IEnumerable<Portfolio>> NewPortfolios = delegate { };

        private void OnNewPortfolios(IEnumerable<Portfolio> portfolios)
        {
            if (NewPortfolios != null) NewPortfolios(portfolios);
        }

        public event Action<Portfolio> PortfolioChanged = delegate { };

        private void OnPortfolioChanged(Portfolio portfolio)
        {
            if (PortfolioChanged != null) PortfolioChanged(portfolio);
        }

        public event Action<Position, eChange> PositionChanged = delegate { };

        private void OnPositionChanged(Position position, eChange change)
        {
            if (PositionChanged != null) PositionChanged(position, change);
        }


        public event Action<Security, bool> NewSecurities = delegate { };

        private void OnNewSecurities(Security security, bool lastEvent)
        {
            if (NewSecurities != null) NewSecurities(security, lastEvent);
        }

        public event Action<Trade> NewTrades = delegate { };

        private void OnNewTrades(Trade trade)
        {
            if (NewTrades != null) NewTrades(trade);
        }

        public event Action<MarketDepth> MarketDepthChanged = delegate { };

        private void OnMarketDepthChanged(MarketDepth depth)
        {
            if (MarketDepthChanged != null) MarketDepthChanged(depth);
        }



        #endregion

        #region Свойства Connector
        /// <summary>
        /// Фильтр списка запрашиваемых инструментов.
        /// </summary>
        public eSecurityType? SecurityTypeLoadFilter { set; get; }

        /// <summary>
        /// Список портфелей.
        /// </summary>
        public List<Portfolio> Portfolios { private set; get; }

        /// <summary>
        /// Список позиций.
        /// </summary>
        public List<Position> Positions { private set; get; }

        public List<Position> ClosedPositions { private set; get; }

        /// <summary>
        /// Список инструментов.
        /// </summary>
        public List<Security> Securities { private set; get; }

        /// <summary>
        /// Инструменты, для которых зарегистрировано получение Level1.
        /// </summary>
        public List<Security> RegisteredSecurities { private set; get; }

        /// <summary>
        /// Инструменты, для которых зарегистрировано получение сделок.
        /// </summary>
        public List<Security> RegisteredTrades { private set; get; }

        /// <summary>
        /// Инструменты, для которых зарегистрировано получение стаканов.
        /// </summary>
        public List<Security> RegisteredMarketDepths { private set; get; }

        /// <summary>
        /// Список заявок.
        /// </summary>
        public List<Order> Orders { private set; get; }

        /// <summary>
        /// Список сделок пользователя.
        /// </summary>
        public List<UserTrade> UserTrades { private set; get; }

        #endregion

        #region Методы Connector


        public void Connect(string login, string password, string ip, ushort port)
        {
            SmartConnect(login, password, ip, port);
        }

        public void Connect(string login, string password, SmartServer server)
        {
            SmartConnect(login, password, server.Address, server.Port);
        }

        public void Connect(string login, string password, eSmartServer server)
        {
            Connect(login, password, SmartServers.GetServer(server));
        }



        public void Disconnect()
        {
            if (!_smartServer.IsConnected()) return;
            _smartServer.disconnect();
        }

        public void RegisterTrades(Security security)
        {
            if (!RegisteredTrades.Contains(security))
            {
                _smartServer.ListenTicks(security.Symbol);
                RegisteredTrades.Add(security);
            }
        }

        public void UnRegisterTrades(Security security)
        {
            if (RegisteredTrades.Contains(security))
            {
                _smartServer.CancelTicks(security.Symbol);
                RegisteredTrades.Remove(security);
            }
        }

        public void RegisterSecurity(Security security)
        {
            if (!RegisteredSecurities.Contains(security))
            {
                _smartServer.ListenQuotes(security.Symbol);
                RegisteredSecurities.Add(security);
            }
        }

        public void UnRegisterSecurity(Security security)
        {
            if (RegisteredSecurities.Contains(security))
            {
                _smartServer.CancelQuotes(security.Symbol);
                RegisteredSecurities.Remove(security);
            }
        }

        public void RegisterMarketDepths(Security security)
        {
            if (!RegisteredMarketDepths.Contains(security))
            {
                _smartServer.ListenBidAsks(security.Symbol);
                RegisteredMarketDepths.Add(security);
            }
        }

        public void UnRegisterMarketDepths(Security security)
        {
            if (RegisteredMarketDepths.Contains(security))
            {
                _smartServer.CancelBidAsks(security.Symbol);
                RegisteredMarketDepths.Remove(security);
            }
        }

        public Order CreateLimitOrder(Portfolio portfolio, Security security, eSide direction, decimal price, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = direction,
                Type = eOrderType.Limit,
                Price = price,
                Volume = volume
            };
        }

        public Order CreateSellLimit(Portfolio portfolio, Security security, decimal price, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Sell,
                Type = eOrderType.Limit,
                Price = price,
                Volume = volume
            };
        }

        public Order CreateBuyLimit(Portfolio portfolio, Security security, decimal price, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Buy,
                Type = eOrderType.Limit,
                Price = price,
                Volume = volume
            };
        }

        public Order CreateBuyMarket(Portfolio portfolio, Security security, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Buy,
                Type = eOrderType.Market,
                Price = 1,
                Volume = volume
            };
        }

        public Order CreateSellMarket(Portfolio portfolio, Security security, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Sell,
                Type = eOrderType.Market,
                Price = 1,
                Volume = volume
            };
        }

        public Order CreateSellStop(Portfolio portfolio, Security security, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Sell,
                Type = eOrderType.Market,
                Price = 1,
                Volume = volume
            };
        }

        public Order CreateBuyStop(Portfolio portfolio, Security security, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Sell,
                Type = eOrderType.Market,
                Price = 1,
                Volume = volume
            };
        }

        public Order CreateSellStopLimit(Portfolio portfolio, Security security, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Sell,
                Type = eOrderType.Market,
                Price = 1,
                Volume = volume
            };
        }

        public Order CreateBuyStopLimit(Portfolio portfolio, Security security, decimal volume)
        {
            ++_lastUserId;

            return new Order()
            {
                UserID = _lastUserId,
                Portfolio = portfolio,
                Security = security,
                Direction = eSide.Sell,
                Type = eOrderType.Market,
                Price = 1,
                Volume = volume
            };
        }

        /// <summary>
        /// Заменить заявку
        /// </summary>
        /// <param name="order">Заменяемая заявка</param>
        /// <param name="price">Цена, на которую будет передвинута заявка.</param>
        public void MoveOrder(Order order, decimal price)
        {
        }

        /// <summary>
        /// Заменить заявку
        /// </summary>
        /// <param name="order">Заменяемая заявка</param>
        /// <param name="shift">Чмсло тиков, на которое будет изменена цена заявки. Положительное число - цена увеличивается, отрицательная - уменьшается.</param>
        public void MoveOrder(Order order, int shift)
        {
        }

        public void SendOrder(Order order)
        {
            PlaceOder(order);
        }

        public void CancelOrder(Order order)
        {
            //TODO
        }

        public void CancelOrders(eSide? direction = null)
        {
            //TODO
        }

        public void CancelOrders(Security security = null, eSide? direction = null)
        {
            //TODO
        }


        #endregion

        #region Обработчики SmartCOM

        // соединение установлено
        private void _smartServer_Connected()
        {

            if (_smartServer.IsConnected())
            {
                try
                {
                    OnConnected();

                    _smartServer.GetPrortfolioList();

                    _smartServer.GetSymbols();
                    
                    // Writers.WriteLine("Enegy", "log", "{0} Get: {1}", DateTime.Now, "Symbols");
                    //SmartServer.GetSymbols();
                }
                catch (COMException ex)
                {
                    // ReDrawStatus("Ошибка при запросе списка инструментов, " + Error.Message);
                }
                catch (Exception ex)
                {
                    //    ReDrawStatus("Ошибка при запросе списка инструментов, " + Error.Message);
                }

                // try
                // {
                //     //Writers.WriteLine("Enegy", "log", "{0} Get: {1}", DateTime.Now, "Prortfolios");
                //     //SmartServer.GetPrortfolioList();                // запросить список доступных счетов

                //    //Writers.WriteLine("Enegy", "log", "{0} Listen: {1}, {2}", DateTime.Now, "Ticks", SymbolTextBox.Text);
                //    //SmartServer.ListenTicks(SymbolTextBox.Text);    // подписаться на получение всех сделок
                //    //Writers.WriteLine("Enegy", "log", "{0} Listen: {1}, {2}", DateTime.Now, "BidAsks", SymbolTextBox.Text);
                //    //SmartServer.ListenBidAsks(SymbolTextBox.Text);  // подписаться на получение стакана
                //    //Writers.WriteLine("Enegy", "log", "{0} Listen: {1}, {2}", DateTime.Now, "Quotes", SymbolTextBox.Text);
                ///    //SmartServer.ListenQuotes(SymbolTextBox.Text);   // подписаться на получение информации по инструменту
                // }
                // catch (Exception ex)
                // {
                //     //ReDrawStatus("Ошибка при подписке, " + Error.Message);
                //     return;
                // }
            }
        }

        // Соединение разорвано
        private void _smartServer_Disconnected(string reason)
        {
            OnDisconnected(reason);
        }

        private void _smartServer_AddPortfolio(int row, int nrows, string portfolioName, string portfolioExch, SmartCOM3Lib.StPortfolioStatus portfolioStatus)
        {
            if (portfolioStatus == StPortfolioStatus.StPortfolioStatus_Broker)
            {
                Portfolios.Add(new Portfolio(portfolioName, portfolioExch));
                _smartServer.ListenPortfolio(portfolioName);
            }
            if (row == nrows) OnNewPortfolios(Portfolios);
        }

        private void _smartServer_SetPortfolio(string portfolio, double cash, double leverage, double comission, double saldo)
        {
            Portfolio prtf = Portfolios.FirstOrDefault(p => p.Name == portfolio);
            if (prtf != null)
            {
                prtf.Update(cash, leverage, comission, saldo);
                OnPortfolioChanged(prtf);
            }
         }

        private void _smartServer_UpdatePosition(string portfolio, string symbol, double avprice, double amount, double planned)
        {
            Position position = Positions.FirstOrDefault(p => p.Security.Code == symbol && p.Portfolio.Name == portfolio);
            if (position != null)
            {
                position.Update(avprice, amount, planned);

                if (position.Volume == 0)
                {
                    if (this.Positions.Contains(position)) this.Positions.Remove(position);
                    OnPositionChanged(position, eChange.Remove);
                }
                else
                {
                    OnPositionChanged(position, eChange.Change);
                }

            }
            else
            {
                position = new Position() { Portfolio = this.GetPortfolio(portfolio), Security = this.Lookup(symbol, "Symbol") };
                position.Update(avprice, amount, planned);
                this.Positions.Add(position);
                OnPositionChanged(position, eChange.New);
            }
        }

        private void _smartServer_AddSymbol(int row, int nrows, string symbol, string short_name, string long_name, string type, int decimals, int lot_size, double punkt, double step, string sec_ext_id, string sec_exch_name, System.DateTime expiry_date, double days_before_expiry, double strike)
        {
            eSecurityType? securityType = Helpers.SmartToSecurityType(type);

            if (securityType != null)
            {
                if (SecurityTypeLoadFilter == null || (SecurityTypeLoadFilter & securityType) == securityType)
                {
                    Security security = Helpers.CreateSecurity(symbol, short_name, long_name, type, decimals, lot_size, punkt, step, sec_ext_id, sec_exch_name, expiry_date, days_before_expiry, strike);

                    if (security != null)
                    {
                        Securities.Add(security);
                        OnNewSecurities(security, row == nrows);
                        return;
                    }

                }
            }

            if (row == nrows) OnNewSecurities(null, true);
        }

        private void _smartServer_AddTick(string symbol, System.DateTime datetime, double price, double volume, string tradeno, SmartCOM3Lib.StOrder_Action action)
        {
            Trade trade = Helpers.CreateTrade(this.Lookup(symbol, "Symbol"), datetime, price, volume, tradeno, action);
            if (trade != null)
            {
                OnNewTrades(trade);
            }
        }


        private Security updateSecurity;

        private void _smartServer_UpdateBidAsk(string symbol, int row, int nrows, double bid, double bidsize, double ask, double asksize)
        {
            if (row == 0)
            {
                updateSecurity = this.Lookup(symbol, "symbol");
                updateSecurity.Depth.Update(row, nrows, bid, bidsize, ask, asksize);
            }
            else if (updateSecurity != null)
            {
                if (row < updateSecurity.Depth.Depth)
                {
                    updateSecurity.Depth.Update(row, nrows, bid, bidsize, ask, asksize);
                }

                if (updateSecurity.Depth.IsUpdate)
                {
                    updateSecurity.Depth.IsUpdate = false;
                    OnMarketDepthChanged(updateSecurity.Depth);
                    updateSecurity = null;
                }
            }

        }

        private void _smartServer_UpdateQuote(string symbol, System.DateTime datetime, double open, double high, double low, double close, double last, double volume, double size, double bid, double ask, double bidsize, double asksize, double open_int, double go_buy, double go_sell, double go_base, double go_base_backed, double high_limit, double low_limit, int trading_status, double volat, double theor_price)
        {
            Security security = this.Lookup(symbol, "symbol");

            if (security != null) security.UpdateQuote(datetime, open, high, low, close, last, volume, size, bid, ask, bidsize,  asksize, open_int, 
                                                        go_buy, go_sell, go_base, go_base_backed, high_limit, low_limit, trading_status, volat, theor_price);
        }


        private void _smartServer_UpdateOrder(string portfolio, string symbol, SmartCOM3Lib.StOrder_State state, SmartCOM3Lib.StOrder_Action action, SmartCOM3Lib.StOrder_Type type, SmartCOM3Lib.StOrder_Validity validity, double price, double amount, double stop, double filled, System.DateTime datetime, string orderid, string orderno, int status_mask, int cookie)
        {
            //TODO
        }

        private void _smartServer_AddTrade(string portfolio, string symbol, string orderid, double price, double amount, System.DateTime datetime, string tradeno)
        {
            //TODO
        }


        private void _smartServer_SetMyClosePos(int row, int nrows, string portfolio, string symbol, double amount, double price_buy, double price_sell, System.DateTime postime, string buy_order, string sell_order)
        {
            //TODO
        }

        private void _smartServer_SetMyOrder(int row, int nrows, string portfolio, string symbol, SmartCOM3Lib.StOrder_State state, SmartCOM3Lib.StOrder_Action action, SmartCOM3Lib.StOrder_Type type, SmartCOM3Lib.StOrder_Validity validity, double price, double amount, double stop, double filled, System.DateTime datetime, string id, string no, int cookie)
        {
            //TODO
        }

        private void _smartServer_SetMyTrade(int row, int nrows, string portfolio, string symbol, System.DateTime datetime, double price, double volume, string tradeno, SmartCOM3Lib.StOrder_Action buysell, string orderno)
        {
            //TODO 
        }

        private void _smartServer_OrderSucceeded(int cookie, string orderid)
        {
            //TODO
        }

        private void _smartServer_OrderFailed(int cookie, string orderid, string reason)
        {
            //TODO
        }

        private void _smartServer_OrderCancelFailed(string orderid)
        {
            //TODO
        }

        private void _smartServer_OrderCancelSucceeded(string orderid)
        {
            //TODO
        }

        private void _smartServer_OrderMoveFailed(string orderid)
        {
            //TODO
        }

        private void _smartServer_OrderMoveSucceeded(string orderid)
        {
            //TODO
        }


        //private void SmartServer_AddBar(int row, int nrows, string symbol, SmartCOM3Lib.StBarInterval interval, System.DateTime datetime, double open, double high, double low, double close, double volume, double open_int)
        //{
        //    if (this.InvokeRequired)
        //        this.BeginInvoke(new MethodInvoker(delegate
        //        {
        //            SmartServer_AddBarInv(row, nrows, symbol, interval, datetime, open,
        //                high, low, close, volume, open_int);
        //        }));
        //    else
        //        SmartServer_AddBarInv(row, nrows, symbol, interval, datetime, open,
        //               high, low, close, volume, open_int);
        //}

        //private void SmartServer_AddBarInv(int row, int nrows, string symbol, SmartCOM3Lib.StBarInterval interval, System.DateTime datetime, double open, double high, double low, double close, double volume, double open_int)
        //{
        //    if (datetime > ToDateTimePicker.Value)  // добавить новый бар в список
        //    {
        //        InfoBars.Add(new Bar(symbol, datetime, open, high, low, close, volume));
        //        InfoBarLabel.Text = datetime.ToShortDateString() + "\n" + datetime.ToLongTimeString() + " (" + InfoBars.Count + ")";
        //        Writers.WriteLine("History", "Bars", "{0}/{1} {2} {3} [O: {4} H: {5} L: {6} C: {7} V: {8} I: {9}] {10}", row.ToString("000;"), nrows.ToString("000;"), datetime, symbol, open, high, low, close, volume, open_int, interval);
        //    }
        //    if (row == nrows - 1) // если пришёл последний бар в запросе
        //    {
        //        ThreadBarsSave();// иначе, считать, что получены все и сохранить список баров
        //    }
        //}

        //private void SmartServer_AddTickHistory(int row, int nrows, string symbol, System.DateTime datetime, double price, double volume, string tradeno, SmartCOM3Lib.StOrder_Action action)
        //{
        //    if (nrows == 0)
        //        Writers.WriteLine("History", "Ticks", "{0} Empty buffer", symbol);
        //    else
        //        Writers.WriteLine("History", "Ticks", "{0}/{1} {2} {3} Price: {4} ({5}) {6} {7}", row, nrows, datetime, symbol, price, volume, tradeno, action);
        //}


    
        #endregion

        #region Private functios

        private void SmartConnect(string login, string password, string ip, ushort port)
        {
            if (_smartServer.IsConnected()) return;
            _smartServer.connect(ip, port, login, password);
        }

        private void PlaceOder(Order order)
        {
            _smartServer.PlaceOrder(order.Portfolio.Name, 
                                    order.Security.Symbol, 
                                    order.Direction.ToSmart().Value, 
                                    order.Type.ToSmart().Value, 
                                    order.Duration.ToSmart().Value, 
                                    (double)order.Price, 
                                    (double)order.Volume, 
                                    (double)order.StopPrice,
                                    order.UserID);
        }

        #endregion


    }
    
    //private delegate void UpdateErrorText(string text);

        //private int InfoCookie;         // Индификатор приказа
        //private Quote LastQuote;        // Котировка инструмента
        //private List<Bar> InfoBars;     // Список баров для инструмента
        //private StServerClass SmartServer;   // SmartCOM
        //private List<Tiker> InfoTikers; // Список всех инструментов
        //private const string smartComParams = "logLevel=5;maxWorkerThreads=3";

        //private DAFWriters Writers;     // Лог

        //// Создан ли SmartCOM
        //private bool IsReady { get { return (SmartServer != null); } }
        //// Установлено ли соединение
        //private bool IsConnected
        //{
        //    get
        //    {
        //        bool bReturn = false;
        //        if (IsReady)
        //        {
        //            try
        //            {
        //                bReturn = SmartServer.IsConnected();
        //            }
        //            catch (Exception)
        //            {

        //            }
        //        }
        //        return bReturn;
        //    }
        //}

        //public TestForm()
        //{
        //    Writers = new DAFWriters();

        //    InitializeComponent();
        //    ToDateTimePicker.Value = DateTime.Now.AddDays(-5);
        //    ToDateTimePicker.MaxDate = DateTime.Now;
        //    InfoBars = new List<Bar>();
        //    InfoTikers = new List<Tiker>();
        //    //Text += ", Version: " + Info.GetVersion;
        //    Writers.WriteLine("Enegy", "log", "{0} Version: {1}", DateTime.Now, Info.GetVersion);

        //    foreach (StBarInterval Interval in Enum.GetValues(typeof(StBarInterval)))
        //        if (Interval != StBarInterval.StBarInterval_Tick)
        //            IntervalComboBox.Items.Add(Interval.ToString());
        //    if (IntervalComboBox.Items.Count > 0)
        //        IntervalComboBox.SelectedIndex = 0;
        //}

        //private StBarInterval GetInterval { get { return (IntervalComboBox.SelectedIndex > -1 ? (StBarInterval)Enum.Parse(typeof(StBarInterval), IntervalComboBox.SelectedItem.ToString()) : StBarInterval.StBarInterval_1Min); } }

        //private void CreateButton_Click(object sender, EventArgs e)
        //{
        //    Writers.WriteLine("Enegy", "log", "{0} Click: {1}", DateTime.Now, "Create");
        //    ReDrawStatus("Create");
        //    if (!IsReady) // если SmartCOM не создан
        //    {
        //        try
        //        {
        //            SmartServer = new StServerClass(); // Создать и назначить обработчики событий

        //            Writers.WriteLine("Enegy", "log", "{0} ConfigureClient {1}", DateTime.Now, smartComParams);
        //            SmartServer.ConfigureClient(smartComParams);
        //            Text = "Test Connect (SmartCOM version: " + SmartServer.GetStClientVersionString() + ")";

        //            CreateButton.Enabled = false;
        //        }
        //        catch (COMException Error)
        //        {
        //            ReDrawStatus("Ошибка при создании, " + Error.Message);
        //            return;
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка при создании, " + Error.Message);
        //            return;
        //        }
        //    }
        //    if (IsConnected) // если соединение установлено, вручную вызвать событие connected, для начала подписки
        //        SmartServer_Connected();
        //    else
        //        ReDrawStatus(""); // иначе обновить статус соединения
        //}
        //private void CreateButton_EnabledChanged(object sender, EventArgs e)
        //{
        //    ConnectButton.Enabled = !CreateButton.Enabled;
        //    DisconnectButton.Enabled = !CreateButton.Enabled;
        //    GetBarsButton.Enabled = !CreateButton.Enabled;
        //    RightPanel.Enabled = !CreateButton.Enabled;
        //}

        //private void ConnectThread()
        //{
        //    if (!IsConnected) // и соединение не установлено
        //    {
        //        try
        //        {
        //            string ip;
        //            ushort port;
        //            // подключится к серверу
        //            if (IPTextBox.Text.IndexOf(":") == -1)
        //            {
        //                ip = IPTextBox.Text;
        //                port = 8090;
        //            }
        //            else
        //            {
        //                ip = IPTextBox.Text.Substring(0, IPTextBox.Text.IndexOf(":"));
        //                port = Convert.ToUInt16(IPTextBox.Text.Substring(IPTextBox.Text.IndexOf(":") + 1));
        //            }

        //            SmartServer.connect(ip, port, LoginTextBox.Text, PasswordTextBox.Text);
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка при подключении, " + Error.Message);
        //        }
        //    }
        //}

        //private void ConnectButton_Click(object sender, EventArgs e)
        //{
        //    Writers.WriteLine("Enegy", "log", "{0} Click: {1}", DateTime.Now, "Connect: " + LoginTextBox.Text);
        //    ReDrawStatus("Connect");
        //    if (IsReady) // если создан SmartCOM
        //    {
        //        Thread connectThread = new Thread(new ThreadStart(ConnectThread));
        //        connectThread.SetApartmentState(ApartmentState.MTA);
        //        connectThread.Start();
        //    }
        //}

        //private void DisconnectButton_Click(object sender, EventArgs e)
        //{
        //    Writers.WriteLine("Enegy", "log", "{0} Click: {1}", DateTime.Now, "Disconnect");
        //    ReDrawStatus("Disconnect");
        //    if (IsConnected)
        //    {
        //        try
        //        {
        //            // отказаться от подписок
        //            SmartServer.CancelTicks(SymbolTextBox.Text);
        //            SmartServer.CancelQuotes(SymbolTextBox.Text);
        //            SmartServer.CancelBidAsks(SymbolTextBox.Text);
        //            foreach (string tempPortfolio in PortfoliosComboBox.Items)
        //                SmartServer.CancelPortfolio(tempPortfolio);
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка при завершении подписки, " + Error.Message);
        //        }
        //        try
        //        {
        //            SmartServer.disconnect();
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка при отключении, " + Error.Message);
        //        }
        //    }
        //}

        //private void GetBarsButton_Click(object sender, EventArgs e)
        //{
        //    ReDrawStatus("");
        //    if (IsConnected && SymbolTextBox.Text != "") // если соединение установлено и указан инструмент
        //    {
        //        GetBarsButton.Enabled = false;
        //        InfoBarLabel.Text = "Start";
        //        InfoBars.Clear(); // Очистить список баров

        //        DateTime LastTime = (LastQuote != null && LastQuote.LastNo > 0 ? DateTime.Parse(LastQuote.LastClock.ToString("yyyy-MM-dd hh:mm") + ":00") : DateTime.Now);
        //        Writers.WriteLine("Enegy", "log", "{0} GetBars 500, LastTick: ", LastTime, (LastQuote != null && LastQuote.LastNo > 0 ? LastQuote.LastClock + " " + LastQuote.LastNo : " UnKnow"));
        //        try
        //        {
        //            // запросить 500 баров начиная с Datetime.Now или от последнего закрытого бара
        //            SmartServer.GetBars(SymbolTextBox.Text, GetInterval,  LastTime, 500);
        //        }
        //        catch (Exception Error)
        //        {
        //            Writers.WriteLine("Enegy", "log", "{0} Ошибка в GetBars {1}", LastTime, Error.Message);
        //        }
        //    }
        //}
        //private void GetBarsButton_EnabledChanged(object sender, EventArgs e)
        //{
        //    ToDateTimePicker.Enabled = GetBarsButton.Enabled;
        //    IntervalComboBox.Enabled = GetBarsButton.Enabled;
        //}

        //private void ReDrawStatus(string reason)
        //{
        //    if (StatusLabel.InvokeRequired) // проверка на главный поток
        //        StatusLabel.BeginInvoke(new UpdateErrorText(ReDrawStatus), reason);
        //    else
        //    {   // Обновить статус соединения
        //        StatusLabel.Text = IsConnected ? "Connected" : IsReady ? "Disconnected" : "No Create";
        //        StatusLabel.ForeColor = IsConnected ? Color.Green : IsReady ? Color.Tomato : Color.DarkGray;
        //    }

        //    if (ErrorLabel.InvokeRequired)
        //        ErrorLabel.BeginInvoke(new UpdateErrorText(ReDrawStatus), reason);
        //    else
        //        ErrorLabel.Text = reason;
        //}

        //private void AddPortfolioToGUI(string portfolioName)
        //{
        //    if (PortfoliosComboBox.InvokeRequired)
        //    {
        //        PortfoliosComboBox.BeginInvoke(new UpdateErrorText(AddPortfolioToGUI), portfolioName);
        //        return;
        //    }

        //    if (PortfoliosComboBox.Items.IndexOf(portfolioName) == -1) // если данный счёт не известен то запомним
        //    {
        //        PortfoliosComboBox.Items.Add(portfolioName);
        //        if (PortfoliosComboBox.SelectedIndex == -1 && PortfoliosComboBox.Items.Count > 0)
        //            PortfoliosComboBox.SelectedIndex = 0;
        //    }
        //}

        //private void UpDateQuote()
        //{
        //    if (LastQuoteLabel.InvokeRequired) // проверка на главный поток
        //        LastQuoteLabel.BeginInvoke(new System.Windows.Forms.MethodInvoker(UpDateQuote));
        //    else
        //    {   // Обновить информацию по инструменту
        //        LastAskLabel.Text = "Ask: " + LastQuote.Ask + " (" + LastQuote.AskVolume + ")";
        //        LastBidLabel.Text = "Bid: " + LastQuote.Bid + " (" + LastQuote.BidVolume + ")";
        //        LastLabel.Text = LastQuote.LastClock.ToLongTimeString() + " " + LastQuote.LastPrice + " (" + LastQuote.LastVolume + ") -> " + (LastQuote.LastAction == StOrder_Action.StOrder_Action_Buy ? "B" : LastQuote.LastAction == StOrder_Action.StOrder_Action_Sell ? "S" : LastQuote.LastAction.ToString());
        //        LastQuoteLabel.Text = "Status: " + LastQuote.Status;
        //    }
        //}

        //private void ThreadBarsSave()
        //{
        //    Writers.WriteLine("GetBars", SymbolTextBox.Text + ".Bars", "*** Save Start {0}", DateTime.Now);
        //    foreach (Bar tempBar in InfoBars)
        //        Writers.WriteLine("GetBars", SymbolTextBox.Text + ".Bars", "{0} -> {1}", InfoBars.IndexOf(tempBar), tempBar.ToString());
        //    Writers.WriteLine("GetBars", SymbolTextBox.Text + ".Bars", "*** Save Finish {0}", DateTime.Now);
        //    if (GetBarsButton.InvokeRequired)
        //        GetBarsButton.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate { GetBarsButton.Enabled = true; }));
        //    else
        //        GetBarsButton.Enabled = true;
        //}

        //private void SetErrorText(string text)
        //{
        //    ErrorLabel.Text = text;
        //}

   

        //private void GetPriceBy_Click(object sender, EventArgs e)
        //{
        //    if (LastQuote != null)
        //    {
        //        // Запомнить цену для выставления или перемещения приказа
        //        if (sender == LastLabel || sender == OrderPriceLabel)
        //            OrderPriceNumericUpDown.Value = System.Convert.ToDecimal(LastQuote.LastPrice);
        //        if (sender == LastAskLabel)
        //            OrderPriceNumericUpDown.Value = System.Convert.ToDecimal(LastQuote.Ask);
        //        if (sender == LastBidLabel)
        //            OrderPriceNumericUpDown.Value = System.Convert.ToDecimal(LastQuote.Bid);
        //        OrderPriceMoveNumericUpDown.Value = OrderPriceNumericUpDown.Value;
        //    }
        //    else
        //        OrderPriceNumericUpDown.Value = 0;
        //}

        //private void PlaceOrder_Click(object sender, EventArgs e)
        //{
        //    if (IsConnected && SymbolTextBox.Text != "" && OrderPriceNumericUpDown.Value > 0 && PortfoliosComboBox.Text != "")
        //    {
        //        InfoCookie++;
        //        OrderCookieLabel.Text = InfoCookie.ToString();
        //        Writers.WriteLine("Portfolio", "Trade", "{0} Order:Place {1} Price: {2}", DateTime.Now, InfoCookie, (double)OrderPriceNumericUpDown.Value);
        //        try
        //        {
        //            // Выставить приказ
        //            SmartServer.PlaceOrder(PortfoliosComboBox.Text, SymbolTextBox.Text, (sender == OrderBuyButton ? StOrder_Action.StOrder_Action_Buy : StOrder_Action.StOrder_Action_Sell), StOrder_Type.StOrder_Type_Limit, StOrder_Validity.StOrder_Validity_Day, (double)OrderPriceNumericUpDown.Value, 1, 0, InfoCookie);
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка при выставлении приказа: " + Error.Message);
        //            Writers.WriteLine("Portfolio", "Trade", "{0} Order:Place {1}, {2}", DateTime.Now, InfoCookie, "Ошибка при выставлении приказа: " + Error.Message);
        //        }
        //    }
        //}

        //private void OrderMoveButton_Click(object sender, EventArgs e)
        //{
        //    if (IsConnected && OrderIdTextBox.Text != "" && OrderPriceMoveNumericUpDown.Value > 0 && PortfoliosComboBox.Text != "")
        //    {
        //        Writers.WriteLine("Portfolio", "Trade", "{0} Order:Move {1} Price: {2}", DateTime.Now, OrderIdTextBox.Text, (double)OrderPriceMoveNumericUpDown.Value);
        //        try
        //        {
        //            // переместить приказ
        //            SmartServer.MoveOrder(PortfoliosComboBox.Text, OrderIdTextBox.Text, (double)OrderPriceMoveNumericUpDown.Value);
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка при перемещении приказа: " + Error.Message);
        //            Writers.WriteLine("Portfolio", "Trade", "{0} Order:Move {1}, {2}", DateTime.Now, OrderIdTextBox.Text, "Ошибка при перемещении приказа: " + Error.Message);
        //        }
        //    }
        //}

        //private void OrderCanselButton_Click(object sender, EventArgs e)
        //{
        //    if (IsConnected && OrderIdTextBox.Text != "" && SymbolTextBox.Text != "" && PortfoliosComboBox.Text != "")
        //    {
        //        Writers.WriteLine("Portfolio", "Trade", "{0} Order:Cansel {1}", DateTime.Now, OrderIdTextBox.Text);
        //        try
        //        {
        //            // Отменить приказ
        //            SmartServer.CancelOrder(PortfoliosComboBox.Text, SymbolTextBox.Text, OrderIdTextBox.Text);
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка при отмене приказа: " + Error.Message);
        //            Writers.WriteLine("Portfolio", "Trade", "{0} Order:Cansel {1}, {2}", DateTime.Now, OrderIdTextBox.Text, "Ошибка при отмене приказа: " + Error.Message);
        //        }
        //    }
        //}

        //private void MoneyAccondFindClick(object sender, EventArgs e)
        //{
        //    if (IsConnected)
        //    {
        //        try
        //        {
        //            // Отменить все приказы
        //            ReDrawStatus(SmartServer.GetMoneyAccount(PortfoliosComboBox.Text));
        //        }
        //        catch (Exception Error)
        //        {
        //            ReDrawStatus("Ошибка поиска денежного счета " + Error.Message);
        //        }
        //    }
        //}

        //private void TestForm_Load(object sender, EventArgs e)
        //{

        //}
}
