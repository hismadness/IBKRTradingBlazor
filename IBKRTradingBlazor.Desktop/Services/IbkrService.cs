using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBKRTradingBlazor.Desktop.Models;

namespace IBKRTradingBlazor.Desktop.Services
{
    public enum TradingMode { Paper, Live }

    public class TradingModeService
    {
        public TradingMode CurrentMode { get; set; } = TradingMode.Paper;
    }

    public class IbkrService
    {
        protected readonly List<PositionInfo> _positions = new();
        protected readonly List<AccountSummaryItem> _accountSummary = new();
        protected readonly List<OrderHistoryItem> _orderHistory = new();

        public event Action<string>? StatusChanged;
        public event Action<List<PositionInfo>>? PositionsUpdated;
        public event Action<List<AccountSummaryItem>>? AccountSummaryUpdated;
        public event Action<List<OrderHistoryItem>>? OrderHistoryUpdated;

        public virtual bool IsConnected { get; protected set; } = false;

        public virtual async Task<bool> ConnectAsync(string host = "127.0.0.1", int port = 7497, int clientId = 11)
        {
            try
            {
                StatusChanged?.Invoke("Connecting to IBKR...");
                
                // Simulate connection delay
                await Task.Delay(1000);
                
                IsConnected = true;
                StatusChanged?.Invoke("Connected to IBKR");
                
                // Load initial data
                await LoadAccountDataAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public virtual async Task DisconnectAsync()
        {
            if (IsConnected)
            {
                StatusChanged?.Invoke("Disconnecting...");
                await Task.Delay(500);
                IsConnected = false;
                StatusChanged?.Invoke("Disconnected");
            }
        }

        public virtual async Task LoadAccountDataAsync()
        {
            if (!IsConnected)
            {
                StatusChanged?.Invoke("Not connected to IBKR");
                return;
            }

            try
            {
                // Simulate loading account data
                await Task.Delay(500);
                
                // Mock account summary data
                _accountSummary.Clear();
                _accountSummary.AddRange(new[]
                {
                    new AccountSummaryItem { Account = "DU123456", Tag = "NetLiquidation", Value = "100000.00", Currency = "USD" },
                    new AccountSummaryItem { Account = "DU123456", Tag = "TotalCashValue", Value = "50000.00", Currency = "USD" },
                    new AccountSummaryItem { Account = "DU123456", Tag = "BuyingPower", Value = "200000.00", Currency = "USD" },
                    new AccountSummaryItem { Account = "DU123456", Tag = "AvailableFunds", Value = "50000.00", Currency = "USD" }
                });
                
                AccountSummaryUpdated?.Invoke(_accountSummary);
                
                // Mock positions data
                _positions.Clear();
                _positions.AddRange(new[]
                {
                    new PositionInfo { Account = "DU123456", Symbol = "AAPL", SecType = "STK", Exchange = "SMART", Currency = "USD", Position = 100, AvgCost = 150.00, MarketPrice = 155.00, ATRFactor = 1.55 }, // TODO: Replace with real calculation
                    new PositionInfo { Account = "DU123456", Symbol = "MSFT", SecType = "STK", Exchange = "SMART", Currency = "USD", Position = 50, AvgCost = 300.00, MarketPrice = 310.00, ATRFactor = 1.24 } // TODO: Replace with real calculation
                });
                
                PositionsUpdated?.Invoke(_positions);
                
                StatusChanged?.Invoke("Account data loaded successfully");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Failed to load account data: {ex.Message}");
            }
        }

        public virtual async Task LoadOrderHistoryAsync()
        {
            if (!IsConnected)
            {
                StatusChanged?.Invoke("Not connected to IBKR");
                return;
            }

            try
            {
                await Task.Delay(500);
                
                // Mock order history data
                _orderHistory.Clear();
                _orderHistory.AddRange(new[]
                {
                    new OrderHistoryItem { OrderId = 1, Symbol = "AAPL", SecType = "STK", Exchange = "SMART", Currency = "USD", Side = "BUY", Shares = 100, Price = 150.00, Time = "2024-01-15 09:30:00", PnL = 500.00 },
                    new OrderHistoryItem { OrderId = 2, Symbol = "MSFT", SecType = "STK", Exchange = "SMART", Currency = "USD", Side = "SELL", Shares = 25, Price = 305.00, Time = "2024-01-14 14:45:00", PnL = -50.00 }
                });
                
                OrderHistoryUpdated?.Invoke(_orderHistory);
                StatusChanged?.Invoke("Order history loaded successfully");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Failed to load order history: {ex.Message}");
            }
        }

        public virtual async Task PlaceOrderAsync(string symbol, string exchange, string secType, string currency, double quantity, double price)
        {
            if (!IsConnected)
            {
                StatusChanged?.Invoke("Not connected to IBKR");
                return;
            }

            try
            {
                StatusChanged?.Invoke($"Placing order: {quantity} {symbol} @ {price}");
                await Task.Delay(1000);
                StatusChanged?.Invoke($"Order placed successfully: {symbol}");
                
                // Refresh account data after order
                await LoadAccountDataAsync();
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Order failed: {ex.Message}");
            }
        }

        public List<PositionInfo> GetPositions() => new List<PositionInfo>(_positions);
        public List<AccountSummaryItem> GetAccountSummary() => new List<AccountSummaryItem>(_accountSummary);
        public List<OrderHistoryItem> GetOrderHistory() => new List<OrderHistoryItem>(_orderHistory);
    }
} 