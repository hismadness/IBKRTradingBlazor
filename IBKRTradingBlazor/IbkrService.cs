using System;
using IBApi; // From IBKR API
using System.Collections.Generic;
using IBKRTradingBlazor.Client.Models;

namespace IBKRTradingBlazor
{
    public class IbkrService
    {
        private EClientSocket _clientSocket;
        private EWrapperImpl _wrapper;
        private bool _isConnected = false;

        public IbkrService()
        {
            _wrapper = new EWrapperImpl();
            _clientSocket = _wrapper.ClientSocket;
        }

        public void Connect(string host = "127.0.0.1", int port = 7497, int clientId = 11)
        {
            if (!_isConnected)
            {
                _clientSocket.eConnect(host, port, clientId);
                _isConnected = true;
            }
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _clientSocket.eDisconnect();
                _isConnected = false;
            }
        }

        public void PlaceOrder(string symbol, string exchange, string secType, string currency, double quantity, double price)
        {
            // This is a placeholder. You would build a Contract and Order object and call _clientSocket.placeOrder
            var contract = new Contract
            {
                Symbol = symbol,
                SecType = secType,
                Exchange = exchange,
                Currency = currency
            };
            var order = new Order
            {
                Action = quantity > 0 ? "BUY" : "SELL",
                OrderType = "LMT",
                TotalQuantity = (decimal)Math.Abs(quantity),
                LmtPrice = price
            };
            _clientSocket.placeOrder(_wrapper.NextOrderId++, contract, order);
        }

        public void RequestPositions()
        {
            _wrapper.ClearPositions();
            _clientSocket.reqPositions();
        }

        public void RequestPositionsWithMarketData()
        {
            RequestPositions();
            _wrapper.RequestMarketDataForPositions(_clientSocket);
        }

        public List<PositionInfo> GetPositions()
        {
            return new List<PositionInfo>(_wrapper.GetPositions());
        }

        public bool PositionsComplete => _wrapper.PositionsComplete;

        public void RequestAccountSummary()
        {
            _wrapper.ClearAccountSummary();
            _clientSocket.reqAccountSummary(9001, "All", "NetLiquidation,TotalCashValue,BuyingPower,AvailableFunds,EquityWithLoanValue,InitMarginReq,MaintMarginReq,ExcessLiquidity,Leverage");
        }

        public List<AccountSummaryItem> GetAccountSummary()
        {
            return new List<AccountSummaryItem>(_wrapper.GetAccountSummary());
        }

        public bool AccountSummaryComplete => _wrapper.AccountSummaryComplete;

        public void RequestOrderHistory()
        {
            _wrapper.ClearOrderHistory();
            var filter = new IBApi.ExecutionFilter();
            _clientSocket.reqExecutions(9002, filter);
        }

        public List<OrderHistoryItem> GetOrderHistory()
        {
            return new List<OrderHistoryItem>(_wrapper.GetOrderHistory());
        }

        public bool OrderHistoryComplete => _wrapper.OrderHistoryComplete;

        public void CancelAllOrders()
        {
            _clientSocket.reqGlobalCancel(new OrderCancel());
        }

        public void ClosePosition(PositionInfo position)
        {
            // Submit a market order in the opposite direction for the position size
            var contract = new Contract
            {
                Symbol = position.Symbol,
                SecType = position.SecType,
                Exchange = position.Exchange,
                Currency = position.Currency
            };
            var order = new Order
            {
                Action = position.Position > 0 ? "SELL" : "BUY",
                OrderType = "MKT",
                TotalQuantity = (decimal)Math.Abs(position.Position)
            };
            _clientSocket.placeOrder(_wrapper.NextOrderId++, contract, order);
        }
    }
}