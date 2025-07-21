namespace IBKRTradingBlazor;

public class TradeRequest
{
    public string? Symbol { get; set; }
    public string? Exchange { get; set; }
    public string? SecType { get; set; }
    public string? Currency { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
} 