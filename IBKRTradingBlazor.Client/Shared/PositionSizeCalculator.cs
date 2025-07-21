using System;

namespace IBKRTradingBlazor.Client.Shared
{
    public class PositionSizeResult
    {
        public int? Quantity { get; set; }
        public double AmountInvested { get; set; }
        public double AmountAtRisk { get; set; }
        public double RiskPercent { get; set; }
        public string? CapHint { get; set; }
        public string? Error { get; set; }
    }

    public static class PositionSizeCalculator
    {
        public static PositionSizeResult Calculate(
            double entryPrice,
            double stopLoss,
            double riskPercent,
            double buyingPower,
            double? avgVolume = null,
            double partialPct = 0
        )
        {
            var result = new PositionSizeResult();
            if (entryPrice <= 0 || stopLoss <= 0)
            {
                result.Error = "Invalid entry or stop loss price.";
                return result;
            }
            double riskPerShare = Math.Abs(entryPrice - stopLoss);
            if (riskPerShare <= 0)
            {
                result.Error = "Risk per share must be positive.";
                return result;
            }
            double maxRiskAmount = buyingPower * (riskPercent / 100.0);
            int quantity = (int)(maxRiskAmount / riskPerShare);
            string capHint = string.Empty;
            if (avgVolume.HasValue)
            {
                int maxQty = (int)(avgVolume.Value * 0.01);
                if (quantity > maxQty)
                {
                    capHint = $" (Capped by 1% of 20d avg volume: {maxQty})";
                    quantity = maxQty;
                }
            }
            if (quantity <= 0)
            {
                result.Error = "Calculated quantity is zero. Check your buying power, risk settings, or stop-loss distance.";
                return result;
            }
            result.Quantity = quantity;
            result.AmountInvested = entryPrice * quantity;
            result.AmountAtRisk = riskPerShare * quantity;
            result.RiskPercent = riskPercent;
            result.CapHint = capHint;
            result.Error = string.Empty;
            return result;
        }
    }
} 