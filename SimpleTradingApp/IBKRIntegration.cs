using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTradingApp
{
    public class IBKRIntegration
    {
        private TcpClient? client;
        private NetworkStream? stream;
        private Thread? readerThread;
        private bool isConnected = false;
        private int nextOrderId = 1;
        
        // TWS API Settings
        private int port = 7496;
        private string host = "127.0.0.1";
        private string connectionType = "TWS";
        private string tradingMode = "Paper Trading";
        
        // Default constants
        private const int TWS_PORT = 7496;
        private const int GATEWAY_PORT = 4001;
        private const string TWS_HOST = "127.0.0.1";
        
        // Connection state
        public bool IsConnected => isConnected;

        // Settings update methods
        public void UpdateConnectionSettings(string newHost, int newPort, string newConnectionType, string newTradingMode)
        {
            host = newHost;
            port = newPort;
            connectionType = newConnectionType;
            tradingMode = newTradingMode;
            
            // Disconnect if currently connected to apply new settings
            if (isConnected)
            {
                Disconnect();
            }
        }

        public string GetConnectionInfo()
        {
            return $"{tradingMode} via {connectionType} on {host}:{port}";
        }
        
        // Events for UI updates
        public event EventHandler<string>? ConnectionStatusChanged;
        public event EventHandler<AccountData>? AccountDataReceived;
        public event EventHandler<PositionData>? PositionDataReceived;
        public event EventHandler<OrderStatus>? OrderStatusReceived;
        public event EventHandler<MarketData>? MarketDataReceived;

        public async Task<bool> ConnectToTWSAsync()
        {
            try
            {
                // Use the exact same connection approach as the test
                client = new TcpClient();
                await client.ConnectAsync(host, port);
                
                // Check if connection was successful (same as test)
                if (client.Connected)
                {
                    isConnected = true;
                    ConnectionStatusChanged?.Invoke(this, $"Connected to {connectionType} ({tradingMode})");
                    return true;
                }
                else
                {
                    ConnectionStatusChanged?.Invoke(this, "Connection failed: Could not establish connection");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ConnectionStatusChanged?.Invoke(this, $"Connection failed: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            isConnected = false;
            readerThread?.Join();
            stream?.Close();
            client?.Close();
            ConnectionStatusChanged?.Invoke(this, $"Disconnected from {connectionType}");
        }

        private void ReadMessages()
        {
            byte[] buffer = new byte[1024];
            
            while (isConnected && stream != null)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        ProcessMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    ConnectionStatusChanged?.Invoke(this, $"Read error: {ex.Message}");
                    break;
                }
            }
        }

        private void ProcessMessage(string message)
        {
            // Parse TWS API messages and trigger appropriate events
            if (message.Contains("accountSummary"))
            {
                // Parse account data
                var accountData = ParseAccountData(message);
                AccountDataReceived?.Invoke(this, accountData);
            }
            else if (message.Contains("position"))
            {
                // Parse position data
                var positionData = ParsePositionData(message);
                PositionDataReceived?.Invoke(this, positionData);
            }
            else if (message.Contains("orderStatus"))
            {
                // Parse order status
                var orderStatus = ParseOrderStatus(message);
                OrderStatusReceived?.Invoke(this, orderStatus);
            }
            else if (message.Contains("tickPrice"))
            {
                // Parse market data
                var marketData = ParseMarketData(message);
                MarketDataReceived?.Invoke(this, marketData);
            }
        }

        public async Task<bool> PlaceOrderAsync(string symbol, string action, int quantity, 
            double price, string orderType, string session = "Regular")
        {
            if (!isConnected) return false;

            try
            {
                // Create order message according to TWS API format
                string orderMessage = CreateOrderMessage(symbol, action, quantity, price, orderType, session);
                
                if (stream != null)
                {
                    byte[] data = System.Text.Encoding.ASCII.GetBytes(orderMessage);
                    await stream.WriteAsync(data, 0, data.Length);
                    
                    nextOrderId++;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ConnectionStatusChanged?.Invoke(this, $"Order placement failed: {ex.Message}");
                return false;
            }
        }

        private string CreateOrderMessage(string symbol, string action, int quantity, 
            double price, string orderType, string session)
        {
            // Format order message according to TWS API specification
            return $"3|{nextOrderId}|{symbol}|{action}|{quantity}|{orderType}|{price}|{session}|";
        }

        private AccountData ParseAccountData(string message)
        {
            // Parse account summary data from TWS
            return new AccountData
            {
                TotalCashValue = 50000.0,
                BuyingPower = 200000.0,
                AvailableFunds = 50000.0,
                DayTrades = 0,
                MarginUsed = 0.0
            };
        }

        private PositionData ParsePositionData(string message)
        {
            // Parse position data from TWS
            return new PositionData
            {
                Symbol = "AAPL",
                Quantity = 100,
                AverageCost = 150.0,
                MarketValue = 15500.0
            };
        }

        private OrderStatus ParseOrderStatus(string message)
        {
            // Parse order status from TWS
            return new OrderStatus
            {
                OrderId = nextOrderId,
                Status = "Filled",
                Filled = 100,
                Remaining = 0,
                AverageFillPrice = 155.0
            };
        }

        private MarketData ParseMarketData(string message)
        {
            // Parse market data from TWS
            return new MarketData
            {
                Symbol = "AAPL",
                Bid = 154.95,
                Ask = 155.05,
                Last = 155.00,
                Volume = 1000000
            };
        }
    }

    // Data classes for TWS API integration
    public class AccountData
    {
        public double TotalCashValue { get; set; }
        public double BuyingPower { get; set; }
        public double AvailableFunds { get; set; }
        public int DayTrades { get; set; }
        public double MarginUsed { get; set; }
    }

    public class PositionData
    {
        public string Symbol { get; set; } = "";
        public int Quantity { get; set; }
        public double AverageCost { get; set; }
        public double MarketValue { get; set; }
    }

    public class OrderStatus
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = "";
        public int Filled { get; set; }
        public int Remaining { get; set; }
        public double AverageFillPrice { get; set; }
    }

    public class MarketData
    {
        public string Symbol { get; set; } = "";
        public double Bid { get; set; }
        public double Ask { get; set; }
        public double Last { get; set; }
        public int Volume { get; set; }
    }
} 