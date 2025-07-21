using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SimpleTradingApp
{
    public static class FormulaEvaluator
    {
        // Safe mathematical functions
        private static readonly Dictionary<string, Func<double[], double>> SafeFunctions = new()
        {
            { "min", args => args.Min() },
            { "max", args => args.Max() },
            { "abs", args => Math.Abs(args[0]) },
            { "round", args => Math.Round(args[0]) },
            { "floor", args => Math.Floor(args[0]) },
            { "ceil", args => Math.Ceiling(args[0]) },
            { "sqrt", args => Math.Sqrt(args[0]) },
            { "pow", args => Math.Pow(args[0], args[1]) },
            { "log", args => Math.Log(args[0]) },
            { "exp", args => Math.Exp(args[0]) }
        };

        // Allowed variable names (add more as needed)
        private static readonly HashSet<string> AllowedVariables = new()
        {
            "entry", "low", "high", "prev_low", "prev_high", "atr", "qty", "buying_power",
            "bid", "ask", "last", "close", "marketPrice", "volume", "avg_volume", "partial_pct",
            "entry_price", "stop_loss", "profit_target", "trailing_stop"
        };

        // Safe operators
        private static readonly Dictionary<string, Func<double, double, double>> SafeOperators = new()
        {
            { "+", (a, b) => a + b },
            { "-", (a, b) => a - b },
            { "*", (a, b) => a * b },
            { "/", (a, b) => b != 0 ? a / b : throw new DivideByZeroException() },
            { "^", (a, b) => Math.Pow(a, b) },
            { "%", (a, b) => a % b }
        };

        public static double EvaluateFormula(string formula, Dictionary<string, double> variables)
        {
            if (string.IsNullOrWhiteSpace(formula))
                throw new ArgumentException("Formula cannot be null or empty");

            try
            {
                // Clean and validate the formula
                formula = CleanFormula(formula);
                
                // Validate variables
                ValidateVariables(formula, variables);
                
                // Parse and evaluate
                return ParseAndEvaluate(formula, variables);
            }
            catch (Exception ex)
            {
                throw new FormulaEvaluationException($"Error evaluating formula '{formula}': {ex.Message}", ex);
            }
        }

        private static string CleanFormula(string formula)
        {
            // Remove extra whitespace and normalize
            formula = Regex.Replace(formula, @"\s+", " ").Trim();
            
            // Replace common aliases
            formula = formula.Replace("entry_price", "entry")
                           .Replace("stop_loss", "entry")
                           .Replace("profit_target", "entry")
                           .Replace("trailing_stop", "entry");
            
            return formula;
        }

        private static void ValidateVariables(string formula, Dictionary<string, double> variables)
        {
            // Extract variable names from formula
            var variablePattern = @"\b[a-zA-Z_][a-zA-Z0-9_]*\b";
            var matches = Regex.Matches(formula, variablePattern);
            
            foreach (Match match in matches)
            {
                var varName = match.Value.ToLower();
                
                // Skip function names and numbers
                if (SafeFunctions.ContainsKey(varName) || IsNumber(varName))
                    continue;
                
                // Check if variable is allowed
                if (!AllowedVariables.Contains(varName))
                    throw new FormulaEvaluationException($"Variable '{varName}' is not allowed");
                
                // Check if variable is provided
                if (!variables.ContainsKey(varName))
                    throw new FormulaEvaluationException($"Variable '{varName}' not provided");
            }
        }

        private static bool IsNumber(string str)
        {
            return double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private static double ParseAndEvaluate(string formula, Dictionary<string, double> variables)
        {
            // Handle simple cases first
            if (IsNumber(formula))
                return double.Parse(formula, CultureInfo.InvariantCulture);

            // Handle function calls
            if (formula.Contains("("))
                return EvaluateFunction(formula, variables);

            // Handle basic arithmetic
            return EvaluateArithmetic(formula, variables);
        }

        private static double EvaluateFunction(string formula, Dictionary<string, double> variables)
        {
            // Match function calls like "min(low, prev_low)" or "abs(entry - low)"
            var functionPattern = @"(\w+)\s*\(([^)]+)\)";
            var match = Regex.Match(formula, functionPattern);
            
            if (!match.Success)
                throw new FormulaEvaluationException($"Invalid function syntax: {formula}");

            var functionName = match.Groups[1].Value.ToLower();
            var arguments = match.Groups[2].Value;

            if (!SafeFunctions.ContainsKey(functionName))
                throw new FormulaEvaluationException($"Function '{functionName}' is not allowed");

            // Parse arguments
            var args = ParseArguments(arguments, variables);
            
            return SafeFunctions[functionName](args);
        }

        private static double[] ParseArguments(string arguments, Dictionary<string, double> variables)
        {
            var args = new List<double>();
            var currentArg = "";
            var parenCount = 0;
            
            for (int i = 0; i < arguments.Length; i++)
            {
                var c = arguments[i];
                
                if (c == '(')
                    parenCount++;
                else if (c == ')')
                    parenCount--;
                else if (c == ',' && parenCount == 0)
                {
                    args.Add(EvaluateArithmetic(currentArg.Trim(), variables));
                    currentArg = "";
                    continue;
                }
                
                currentArg += c;
            }
            
            if (!string.IsNullOrWhiteSpace(currentArg))
                args.Add(EvaluateArithmetic(currentArg.Trim(), variables));
            
            return args.ToArray();
        }

        private static double EvaluateArithmetic(string expression, Dictionary<string, double> variables)
        {
            // Replace variables with their values
            foreach (var variable in variables)
            {
                expression = Regex.Replace(expression, $@"\b{variable.Key}\b", variable.Value.ToString(CultureInfo.InvariantCulture));
            }

            // Evaluate the expression using a simple stack-based calculator
            return EvaluateExpression(expression);
        }

        private static double EvaluateExpression(string expression)
        {
            var tokens = Tokenize(expression);
            var postfix = ConvertToPostfix(tokens);
            return EvaluatePostfix(postfix);
        }

        private static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            var current = "";
            
            for (int i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                
                if (char.IsDigit(c) || c == '.')
                {
                    current += c;
                }
                else if (char.IsLetter(c))
                {
                    current += c;
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '%')
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        tokens.Add(current);
                        current = "";
                    }
                    tokens.Add(c.ToString());
                }
                else if (c == '(' || c == ')')
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        tokens.Add(current);
                        current = "";
                    }
                    tokens.Add(c.ToString());
                }
            }
            
            if (!string.IsNullOrEmpty(current))
                tokens.Add(current);
            
            return tokens;
        }

        private static List<string> ConvertToPostfix(List<string> tokens)
        {
            var output = new List<string>();
            var operators = new Stack<string>();
            var precedence = new Dictionary<string, int>
            {
                { "+", 1 }, { "-", 1 },
                { "*", 2 }, { "/", 2 }, { "%", 2 },
                { "^", 3 }
            };

            foreach (var token in tokens)
            {
                if (IsNumber(token))
                {
                    output.Add(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Add(operators.Pop());
                    }
                    if (operators.Count > 0 && operators.Peek() == "(")
                        operators.Pop();
                }
                else if (precedence.ContainsKey(token))
                {
                    while (operators.Count > 0 && operators.Peek() != "(" &&
                           precedence[operators.Peek()] >= precedence[token])
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return output;
        }

        private static double EvaluatePostfix(List<string> postfix)
        {
            var stack = new Stack<double>();

            foreach (var token in postfix)
            {
                if (IsNumber(token))
                {
                    stack.Push(double.Parse(token, CultureInfo.InvariantCulture));
                }
                else
                {
                    var b = stack.Pop();
                    var a = stack.Pop();
                    stack.Push(SafeOperators[token](a, b));
                }
            }

            return stack.Pop();
        }

        public static bool IsValidFormula(string formula)
        {
            try
            {
                // Test with dummy variables
                var dummyVariables = new Dictionary<string, double>
                {
                    { "entry", 100.0 },
                    { "low", 95.0 },
                    { "high", 105.0 },
                    { "atr", 2.0 },
                    { "qty", 100 },
                    { "buying_power", 10000.0 }
                };

                EvaluateFormula(formula, dummyVariables);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class FormulaEvaluationException : Exception
    {
        public FormulaEvaluationException(string message) : base(message) { }
        public FormulaEvaluationException(string message, Exception innerException) : base(message, innerException) { }
    }
} 