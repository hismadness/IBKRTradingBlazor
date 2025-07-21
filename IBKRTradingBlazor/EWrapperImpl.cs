using System;
using System.Collections.Generic;
using IBApi;
using IBApi.protobuf;
using IBKRTradingBlazor.Client.Models;

namespace IBKRTradingBlazor
{
    public class EWrapperImpl : EWrapper
    {
        public EClientSocket ClientSocket { get; }
        public int NextOrderId { get; set; } = 1;
        private readonly List<PositionInfo> _positions = new();
        private bool _positionsComplete = false;
        public IReadOnlyList<PositionInfo> GetPositions() => _positions.AsReadOnly();
        public void ClearPositions() { _positions.Clear(); _positionsComplete = false; }
        public bool PositionsComplete => _positionsComplete;

        private readonly List<AccountSummaryItem> _accountSummary = new();
        private bool _accountSummaryComplete = false;
        public IReadOnlyList<AccountSummaryItem> GetAccountSummary() => _accountSummary.AsReadOnly();
        public void ClearAccountSummary() { _accountSummary.Clear(); _accountSummaryComplete = false; }
        public bool AccountSummaryComplete => _accountSummaryComplete;

        private readonly List<OrderHistoryItem> _orderHistory = new();
        private bool _orderHistoryComplete = false;
        public IReadOnlyList<OrderHistoryItem> GetOrderHistory() => _orderHistory.AsReadOnly();
        public void ClearOrderHistory() { _orderHistory.Clear(); _orderHistoryComplete = false; }
        public bool OrderHistoryComplete => _orderHistoryComplete;

        private readonly Dictionary<string, OrderHistoryItem> _openTrades = new();

        public EWrapperImpl()
        {
            ClientSocket = new EClientSocket(this, new EReaderMonitorSignal());
        }

        public void error(Exception e) { }
        public void error(string str) { }
        public void error(int id, long errorTime, int errorCode, string errorMsg, string advancedOrderRejectJson) { }
        public void currentTime(long time) { }
        public void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            // Update MarketPrice for positions if symbol matches
            if (field == 4) // Last price
            {
                foreach (var pos in _positions)
                {
                    if (TickerIdForSymbol(pos.Symbol) == tickerId)
                    {
                        pos.MarketPrice = price;
                    }
                }
            }
        }
        private int TickerIdForSymbol(string symbol)
        {
            // Simple mapping: hash code (for demo, should be improved for real use)
            return symbol.GetHashCode();
        }
        public void RequestMarketDataForPositions(EClientSocket client)
        {
            foreach (var pos in _positions)
            {
                client.reqMktData(TickerIdForSymbol(pos.Symbol), new IBApi.Contract
                {
                    Symbol = pos.Symbol,
                    SecType = pos.SecType,
                    Exchange = pos.Exchange,
                    Currency = pos.Currency
                }, "", false, false, null);
            }
        }
        public void tickSize(int tickerId, int field, decimal size) { }
        public void tickString(int tickerId, int field, string value) { }
        public void tickGeneric(int tickerId, int field, double value) { }
        public void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate) { }
        public void deltaNeutralValidation(int reqId, IBApi.DeltaNeutralContract deltaNeutralContract) { }
        public void tickOptionComputation(int tickerId, int field, int tickAttrib, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice) { }
        public void tickSnapshotEnd(int tickerId) { }
        public void nextValidId(int orderId) { NextOrderId = orderId; }
        public void managedAccounts(string accountsList) { }
        public void connectionClosed() { }
        public void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            _accountSummary.Add(new AccountSummaryItem
            {
                Account = account,
                Tag = tag,
                Value = value,
                Currency = currency
            });
        }
        public void accountSummaryEnd(int reqId)
        {
            _accountSummaryComplete = true;
        }
        public void updateAccountValue(string key, string value, string currency, string accountName) { }
        public void updatePortfolio(IBApi.Contract contract, decimal position, double marketPrice, double marketValue, double averageCost, double unrealizedPNL, double realizedPNL, string accountName) { }
        public void updateAccountTime(string timestamp) { }
        public void accountDownloadEnd(string account) { }
        public void orderStatus(int orderId, string status, decimal filled, decimal remaining, double avgFillPrice, long permId, int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice) { }
        public void openOrder(int orderId, IBApi.Contract contract, IBApi.Order order, IBApi.OrderState orderState) { }
        public void openOrderEnd() { }
        public void contractDetails(int reqId, ContractDetails contractDetails) { }
        public void contractDetailsEnd(int reqId) { }
        public void execDetails(int reqId, IBApi.Contract contract, IBApi.Execution execution)
        {
            var key = contract.Symbol + ":" + execution.Side;
            var oppositeSide = execution.Side == "BUY" ? "SELL" : "BUY";
            var oppKey = contract.Symbol + ":" + oppositeSide;
            // If this is an opening trade, store it
            if (!_openTrades.ContainsKey(key))
            {
                var openTrade = new OrderHistoryItem
                {
                    OrderId = execution.OrderId,
                    Symbol = contract.Symbol,
                    SecType = contract.SecType,
                    Exchange = contract.Exchange,
                    Currency = contract.Currency,
                    Side = execution.Side,
                    Shares = (double)execution.Shares,
                    Price = execution.Price,
                    Time = execution.Time,
                    PnL = 0 // Will be set on close
                };
                _openTrades[key] = openTrade;
                _orderHistory.Add(openTrade);
            }
            else // Closing trade
            {
                var openTrade = _openTrades[key];
                double pnl = 0;
                if (execution.Side == "SELL")
                {
                    pnl = (execution.Price - openTrade.Price) * (double)execution.Shares;
                }
                else // BUY to cover short
                {
                    pnl = (openTrade.Price - execution.Price) * (double)execution.Shares;
                }
                var closeTrade = new OrderHistoryItem
                {
                    OrderId = execution.OrderId,
                    Symbol = contract.Symbol,
                    SecType = contract.SecType,
                    Exchange = contract.Exchange,
                    Currency = contract.Currency,
                    Side = execution.Side,
                    Shares = (double)execution.Shares,
                    Price = execution.Price,
                    Time = execution.Time,
                    PnL = pnl
                };
                _orderHistory.Add(closeTrade);
                _openTrades.Remove(key);
            }
        }
        public void execDetailsEnd(int reqId)
        {
            _orderHistoryComplete = true;
        }
        public void commissionAndFeesReport(CommissionAndFeesReport commissionAndFeesReport) { }
        public void fundamentalData(int reqId, string data) { }
        public void historicalData(int reqId, Bar bar) { }
        public void historicalDataUpdate(int reqId, Bar bar) { }
        public void historicalDataEnd(int reqId, string start, string end) { }
        public void marketDataType(int reqId, int marketDataType) { }
        public void updateMktDepth(int tickerId, int position, int operation, int side, double price, decimal size) { }
        public void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, decimal size, bool isSmartDepth) { }
        public void updateNewsBulletin(int msgId, int msgType, string message, string origExchange) { }
        public void position(string account, IBApi.Contract contract, decimal pos, double avgCost)
        {
            if (pos != 0)
            {
                _positions.Add(new PositionInfo
                {
                    Account = account,
                    Symbol = contract.Symbol,
                    SecType = contract.SecType,
                    Exchange = contract.Exchange,
                    Currency = contract.Currency,
                    Position = pos,
                    AvgCost = avgCost
                });
            }
        }
        public void positionEnd()
        {
            _positionsComplete = true;
        }
        public void realtimeBar(int reqId, long date, double open, double high, double low, double close, decimal volume, decimal WAP, int count) { }
        public void scannerParameters(string xml) { }
        public void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr) { }
        public void scannerDataEnd(int reqId) { }
        public void receiveFA(int faDataType, string faXmlData) { }
        public void verifyMessageAPI(string apiData) { }
        public void verifyCompleted(bool isSuccessful, string errorText) { }
        public void verifyAndAuthMessageAPI(string apiData, string xyzChallenge) { }
        public void verifyAndAuthCompleted(bool isSuccessful, string errorText) { }
        public void displayGroupList(int reqId, string groups) { }
        public void displayGroupUpdated(int reqId, string contractInfo) { }
        public void connectAck() { }
        public void positionMulti(int requestId, string account, string modelCode, IBApi.Contract contract, decimal pos, double avgCost) { }
        public void positionMultiEnd(int requestId) { }
        public void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency) { }
        public void accountUpdateMultiEnd(int requestId) { }
        public void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes) { }
        public void securityDefinitionOptionParameterEnd(int reqId) { }
        public void softDollarTiers(int reqId, IBApi.SoftDollarTier[] tiers) { }
        public void familyCodes(FamilyCode[] familyCodes) { }
        public void symbolSamples(int reqId, IBApi.ContractDescription[] contractDescriptions) { }
        public void mktDepthExchanges(IBApi.DepthMktDataDescription[] depthMktDataDescriptions) { }
        public void tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData) { }
        public void smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap) { }
        public void tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions) { }
        public void newsProviders(NewsProvider[] newsProviders) { }
        public void newsArticle(int requestId, int articleType, string articleText) { }
        public void historicalNews(int requestId, string time, string providerCode, string articleId, string headline) { }
        public void historicalNewsEnd(int requestId, bool hasMore) { }
        public void headTimestamp(int reqId, string headTimestamp) { }
        public void histogramData(int reqId, HistogramEntry[] data) { }
        public void rerouteMktDataReq(int reqId, int conid, string exchange) { }
        public void rerouteMktDepthReq(int reqId, int conid, string exchange) { }
        public void marketRule(int marketRuleId, PriceIncrement[] priceIncrements) { }
        public void pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL) { }
        public void pnlSingle(int reqId, decimal pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value) { }
        public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done) { }
        public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done) { }
        public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done) { }
        public void tickByTickAllLast(int reqId, int tickType, long time, double price, decimal size, TickAttribLast tickAttribLast, string exchange, string specialConditions) { }
        public void tickByTickBidAsk(int reqId, long time, double bidPrice, double askPrice, decimal bidSize, decimal askSize, TickAttribBidAsk tickAttribBidAsk) { }
        public void tickByTickMidPoint(int reqId, long time, double midPoint) { }
        public void orderBound(long permId, int clientId, int orderId) { }
        public void completedOrder(IBApi.Contract contract, IBApi.Order order, IBApi.OrderState orderState) { }
        public void completedOrdersEnd() { }
        public void replaceFAEnd(int reqId, string text) { }
        public void wshMetaData(int reqId, string dataJson) { }
        public void wshEventData(int reqId, string dataJson) { }
        public void historicalSchedule(int reqId, string startDateTime, string endDateTime, string timeZone, HistoricalSession[] sessions) { }
        public void userInfo(int reqId, string whiteBrandingId) { }
        public void currentTimeInMillis(long timeInMillis) { }
        public void orderStatusProtoBuf(OrderStatus orderStatusProto) { }
        public void openOrderProtoBuf(OpenOrder openOrderProto) { }
        public void openOrdersEndProtoBuf(OpenOrdersEnd openOrdersEndProto) { }
        public void errorProtoBuf(ErrorMessage errorMessageProto) { }
        public void execDetailsProtoBuf(ExecutionDetails executionDetailsProto) { }
        public void execDetailsEndProtoBuf(ExecutionDetailsEnd executionDetailsEndProto) { }
        public void bondContractDetails(int reqId, IBApi.ContractDetails contractDetails) { }
    }
}