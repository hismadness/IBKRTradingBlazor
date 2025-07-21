namespace IBKRTradingBlazor.Client.Models;

public class PositionInfo
{
    public string? Account { get; set; }
    public string? Symbol { get; set; }
    public string? SecType { get; set; }
    public string? Exchange { get; set; }
    public string? Currency { get; set; }
    public decimal Position { get; set; }
    public double AvgCost { get; set; }
    public double MarketPrice { get; set; }
    public double PercentGain => AvgCost == 0 ? 0 : ((MarketPrice - AvgCost) / AvgCost) * 100 * (Position > 0 ? 1 : -1);
    public double UsdGain => (MarketPrice - AvgCost) * (double)Position;
}

public class AccountSummaryItem
{
    public string? Account { get; set; }
    public string? Tag { get; set; }
    public string? Value { get; set; }
    public string? Currency { get; set; }
}

public class OrderHistoryItem
{
    public int OrderId { get; set; }
    public string? Symbol { get; set; }
    public string? SecType { get; set; }
    public string? Exchange { get; set; }
    public string? Currency { get; set; }
    public string? Side { get; set; }
    public double Shares { get; set; }
    public double Price { get; set; }
    public string? Time { get; set; }
    public double PnL { get; set; }
} 