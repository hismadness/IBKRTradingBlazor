using System;
using System.Collections.Generic;
using System.Linq;

namespace IBKRTradingBlazor.Desktop.Models
{
    public class TradeOutcome
    {
        public double PnL { get; set; } // Profit and Loss for the trade
    }

    public static class RiskManager
    {
        public static double GetCurrentRiskPercent(List<TradeOutcome> history)
        {
            // Start at 0.25%
            double[] riskSteps = { 0.25, 0.5, 1.0 };
            int riskIndex = 0;
            int consecutiveWins = 0;
            int consecutiveLosses = 0;

            // Analyze from oldest to newest
            foreach (var trade in history)
            {
                if (trade.PnL >= 0)
                {
                    consecutiveWins++;
                    consecutiveLosses = 0;
                    if (consecutiveWins == 2 && riskIndex < riskSteps.Length - 1)
                    {
                        riskIndex++;
                        consecutiveWins = 0;
                    }
                }
                else
                {
                    consecutiveLosses++;
                    consecutiveWins = 0;
                    if (consecutiveLosses == 2 && riskIndex > 0)
                    {
                        riskIndex--;
                        consecutiveLosses = 0;
                    }
                }
            }
            return riskSteps[riskIndex];
        }
    }
} 