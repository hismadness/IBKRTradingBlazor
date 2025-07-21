using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO; // Added for File operations
using System.Linq; // Added for FirstOrDefault

namespace SimpleTradingApp
{
    public class TradingRule
    {
        public string Name { get; set; } = "";
        public string StopLoss { get; set; } = "";
        public string TrailingStop { get; set; } = "";
        public string ProfitTaking { get; set; } = "";
        public double PartialPct { get; set; } = 0.0;
        public string ExitRule { get; set; } = "";
        public bool Selected { get; set; } = false;
    }

    public static class TradingRules
    {
        private static readonly List<TradingRule> DefaultRules = new List<TradingRule>
        {
            new TradingRule
            {
                Name = "Dead Cat",
                StopLoss = "low",
                TrailingStop = "",
                ProfitTaking = "vwap",
                PartialPct = 0.0,
                ExitRule = "",
                Selected = true
            },
            new TradingRule
            {
                Name = "Arndt Daily 1/3 Breakeven",
                StopLoss = "low",
                TrailingStop = "",
                ProfitTaking = "entry * 1.05",
                PartialPct = 33.3,
                ExitRule = "move_stop_to_breakeven_after_partial",
                Selected = true
            },
            new TradingRule
            {
                Name = "Arndt Weekly 1/3 Breakeven",
                StopLoss = "low_of_week",
                TrailingStop = "",
                ProfitTaking = "entry * 1.05",
                PartialPct = 33.3,
                ExitRule = "move_stop_to_breakeven_after_partial",
                Selected = true
            },
            new TradingRule
            {
                Name = "Basic Arndt",
                StopLoss = "low if entry_type == 'Long' else high",
                TrailingStop = "atr * 1.5",
                ProfitTaking = "entry + (8 * atr) if entry_type == 'Long' else entry - (tp_mult * atr)",
                PartialPct = 0.0,
                ExitRule = "",
                Selected = false
            }
        };

        public static List<TradingRule> LoadRules()
        {
            try
            {
                if (File.Exists("trade_rules.json"))
                {
                    string json = File.ReadAllText("trade_rules.json");
                    var rules = JsonSerializer.Deserialize<List<TradingRule>>(json);
                    return rules ?? DefaultRules;
                }
            }
            catch (Exception)
            {
                // If file doesn't exist or is corrupted, return default rules
            }
            
            // Save default rules if file doesn't exist
            SaveRules(DefaultRules);
            return DefaultRules;
        }

        public static void SaveRules(List<TradingRule> rules)
        {
            try
            {
                string json = JsonSerializer.Serialize(rules, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("trade_rules.json", json);
            }
            catch (Exception)
            {
                // Handle save error silently
            }
        }

        public static TradingRule? GetSelectedRule()
        {
            var rules = LoadRules();
            return rules.FirstOrDefault(r => r.Selected);
        }

        public static double EvaluateStopLoss(TradingRule rule, double entryPrice, string entryType, double low = 0, double high = 0)
        {
            if (string.IsNullOrEmpty(rule.StopLoss))
            {
                return entryType == "Long" ? low : high;
            }

            try
            {
                // Use advanced formula evaluator
                var variables = new Dictionary<string, double>
                {
                    { "entry", entryPrice },
                    { "low", low },
                    { "high", high },
                    { "entry_type", entryType == "Long" ? 1 : -1 }
                };

                return FormulaEvaluator.EvaluateFormula(rule.StopLoss, variables);
            }
            catch (FormulaEvaluationException ex)
            {
                // Fallback to simple evaluation
                string formula = rule.StopLoss.ToLower();
                
                if (formula.Contains("low"))
                {
                    return low;
                }
                else if (formula.Contains("high"))
                {
                    return high;
                }
                else if (formula.Contains("entry"))
                {
                    return entryPrice;
                }
                
                // Default fallback
                return entryType == "Long" ? low : high;
            }
        }

        public static double EvaluateProfitTaking(TradingRule rule, double entryPrice, double atr = 0)
        {
            if (string.IsNullOrEmpty(rule.ProfitTaking))
            {
                return 0; // No profit taking
            }

            try
            {
                // Use advanced formula evaluator
                var variables = new Dictionary<string, double>
                {
                    { "entry", entryPrice },
                    { "atr", atr }
                };

                return FormulaEvaluator.EvaluateFormula(rule.ProfitTaking, variables);
            }
            catch (FormulaEvaluationException ex)
            {
                // Fallback to simple evaluation
                string formula = rule.ProfitTaking.ToLower();
                
                if (formula.Contains("entry * 1.05"))
                {
                    return entryPrice * 1.05;
                }
                else if (formula.Contains("entry + (8 * atr)"))
                {
                    return entryPrice + (8 * atr);
                }
                else if (formula.Contains("vwap"))
                {
                    // Simplified VWAP calculation
                    return entryPrice * 1.02; // Approximate VWAP
                }
                
                return 0; // Default: no profit taking
            }
        }

        public static double EvaluateTrailingStop(TradingRule rule, double atr = 0)
        {
            if (string.IsNullOrEmpty(rule.TrailingStop))
            {
                return 0; // No trailing stop
            }

            try
            {
                // Use advanced formula evaluator
                var variables = new Dictionary<string, double>
                {
                    { "atr", atr }
                };

                return FormulaEvaluator.EvaluateFormula(rule.TrailingStop, variables);
            }
            catch (FormulaEvaluationException ex)
            {
                // Fallback to simple evaluation
                string formula = rule.TrailingStop.ToLower();
                
                if (formula.Contains("atr * 1.5"))
                {
                    return atr * 1.5;
                }
                else if (formula.Contains("atr13"))
                {
                    return atr * 1.3; // Simplified 13-day ATR
                }
                
                return 0; // Default: no trailing stop
            }
        }
    }
} 