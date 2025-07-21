using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Added for Skip and Take

namespace SimpleTradingApp
{
    public interface IMarketDataService
    {
        Task<RealTimeMarketData> GetMarketDataAsync(string symbol);
        Task<double> GetATRAsync(string symbol, int period = 14);
        Task<double> GetAverageVolumeAsync(string symbol, int days = 20);
        Task<List<HistoricalBar>> GetHistoricalDataAsync(string symbol, string duration = "2 D", string barSize = "1 day");
        Task<double> GetBuyingPowerAsync();
        Task<bool> IsConnectedAsync();
        event EventHandler<MarketDataUpdateEventArgs> MarketDataUpdated;
        Task<double> GetSMAAsync(string symbol, int period = 50);
    }

    public class RealTimeMarketData
    {
        public string Symbol { get; set; } = "";
        public double? Last { get; set; }
        public double? Bid { get; set; }
        public double? Ask { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public double? Close { get; set; }
        public double? Open { get; set; }
        public long? Volume { get; set; }
        public DateTime Timestamp { get; set; }
        public string Error { get; set; } = "";
        public bool HasError => !string.IsNullOrEmpty(Error);

        public double MarketPrice => Last ?? Bid ?? Ask ?? Close ?? 0.0;
    }

    public class HistoricalBar
    {
        public DateTime Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
    }

    public class MarketDataUpdateEventArgs : EventArgs
    {
        public string Symbol { get; set; } = "";
        public RealTimeMarketData MarketData { get; set; } = new();
        public string Field { get; set; } = "";
        public double? UpdatedValue { get; set; }
    }

    // Simulated market data service for development
    public class SimulatedMarketDataService : IMarketDataService
    {
        private readonly Dictionary<string, RealTimeMarketData> _marketDataCache = new();
        private readonly Random _random = new Random();

        public event EventHandler<MarketDataUpdateEventArgs>? MarketDataUpdated;

        public async Task<RealTimeMarketData> GetMarketDataAsync(string symbol)
        {
            try
            {
                // Use cancellation token to prevent infinite delays
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                
                // Reduced delay to prevent UI freezing
                await Task.Delay(10, cts.Token);

                if (!_marketDataCache.ContainsKey(symbol))
                {
                    // Create simulated market data
                    var basePrice = 100.0 + (_random.NextDouble() * 200); // Random price between 100-300
                    var volatility = basePrice * 0.02; // 2% volatility

                    _marketDataCache[symbol] = new RealTimeMarketData
                    {
                        Symbol = symbol,
                        Last = basePrice,
                        Bid = basePrice - (volatility * 0.5),
                        Ask = basePrice + (volatility * 0.5),
                        High = basePrice + volatility,
                        Low = basePrice - volatility,
                        Close = basePrice,
                        Open = basePrice - (volatility * 0.3),
                        Volume = _random.Next(100000, 1000000),
                        Timestamp = DateTime.Now
                    };
                }

                // Simulate price movement
                var data = _marketDataCache[symbol];
                var priceChange = (_random.NextDouble() - 0.5) * data.Last!.Value * 0.01; // ±0.5% change
                data.Last += priceChange;
                data.Bid = data.Last - (data.Last * 0.001);
                data.Ask = data.Last + (data.Last * 0.001);
                data.Timestamp = DateTime.Now;

                // Trigger update event on background thread to prevent UI blocking
                _ = Task.Run(() =>
                {
                    try
                    {
                        MarketDataUpdated?.Invoke(this, new MarketDataUpdateEventArgs
                        {
                            Symbol = symbol,
                            MarketData = data,
                            Field = "last",
                            UpdatedValue = data.Last
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't crash
                        System.Diagnostics.Debug.WriteLine($"Market data update error: {ex.Message}");
                    }
                });

                return data;
            }
            catch (OperationCanceledException)
            {
                return new RealTimeMarketData
                {
                    Symbol = symbol,
                    Error = "Market data request timed out"
                };
            }
            catch (Exception ex)
            {
                return new RealTimeMarketData
                {
                    Symbol = symbol,
                    Error = ex.Message
                };
            }
        }

        public async Task<double> GetATRAsync(string symbol, int period = 14)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                await Task.Delay(5, cts.Token);
                var data = await GetMarketDataAsync(symbol);
                return data.High!.Value - data.Low!.Value; // Simplified ATR
            }
            catch (OperationCanceledException)
            {
                return 2.0; // Default ATR value
            }
        }

        public async Task<double> GetAverageVolumeAsync(string symbol, int days = 20)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                await Task.Delay(5, cts.Token);
                var data = await GetMarketDataAsync(symbol);
                return data.Volume ?? 500000; // Return volume or default
            }
            catch (OperationCanceledException)
            {
                return 500000.0; // Default volume
            }
        }

        public async Task<List<HistoricalBar>> GetHistoricalDataAsync(string symbol, string duration = "2 D", string barSize = "1 day")
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await Task.Delay(20, cts.Token);
                
                var bars = new List<HistoricalBar>();
                var basePrice = 100.0 + (_random.NextDouble() * 200);
                var currentTime = DateTime.Now.AddDays(-2);

                for (int i = 0; i < 2; i++)
                {
                    var volatility = basePrice * 0.02;
                    var open = basePrice + (_random.NextDouble() - 0.5) * volatility;
                    var high = open + (_random.NextDouble() * volatility);
                    var low = open - (_random.NextDouble() * volatility);
                    var close = open + (_random.NextDouble() - 0.5) * volatility;

                    bars.Add(new HistoricalBar
                    {
                        Time = currentTime.AddDays(i),
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = _random.Next(100000, 1000000)
                    });

                    basePrice = close; // Use close as next day's base
                }

                return bars;
            }
            catch (OperationCanceledException)
            {
                return new List<HistoricalBar>(); // Return empty list on timeout
            }
        }

        public async Task<double> GetBuyingPowerAsync()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                await Task.Delay(5, cts.Token);
                return 100000.0; // Simulated buying power
            }
            catch (OperationCanceledException)
            {
                return 100000.0; // Default buying power
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                await Task.Delay(2, cts.Token);
                return true; // Always connected in simulation
            }
            catch (OperationCanceledException)
            {
                return false; // Not connected on timeout
            }
        }

        public async Task<double> GetSMAAsync(string symbol, int period = 50)
        {
            var bars = await GetHistoricalDataAsync(symbol, "60 D", "1 day");
            if (bars.Count < period) return 0;
            return bars.Skip(bars.Count - period).Take(period).Average(b => b.Close);
        }
    }
} 