using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Threading.Tasks;

namespace SimpleTradingApp
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new TradingForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception in Main: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    public class TradingForm : Form
    {
        // UI Components
        private Panel mainContainer = null!;
        private Panel sidebarPanel = null!;
        private Panel contentPanel = null!;
        private Panel headerPanel = null!;
        private Panel chartPanel = null!;
        private Panel orderPanel = null!;
        private Panel watchlistPanel = null!;
        private Panel accountPanel = null!;
        
        // Controls
        private Label titleLabel = null!;
        private Button connectButton = null!;
        private Button disconnectButton = null!;
        private TextBox symbolTextBox = null!;
        private ComboBox orderTypeComboBox = null!;
        private TextBox quantityTextBox = null!;
        private TextBox priceTextBox = null!;
        private Button buyButton = null!;
        private Button sellButton = null!;
        private ListView positionsListView = null!;
        private ListView accountListView = null!;
        private Label statusLabel = null!;
        private Label balanceLabel = null!;
        private Label pnlLabel = null!;
        private Button settingsButton = null!;
        
        // Data
        private List<Trade> tradeHistory = new List<Trade>();
        private Dictionary<string, decimal> positions = new Dictionary<string, decimal>();
        private Timer updateTimer = null!;
        private bool isConnected = false;
        private bool isLiveTrading = false;
        private IMarketDataService marketDataService = new SimulatedMarketDataService(); // Replace with real as needed

        private string host = "127.0.0.1";
        private int port = 7497;
        private int clientId = 11;

        // Add these fields to TradingForm:
        private Panel tradingRulesRibbon = null!;
        private Panel tradingPanel = null!;
        private Panel accountSummaryPanel = null!;
        private Panel positionsPanel = null!;

        public TradingForm()
        {
            InitializeComponent();
            CreateLayout();
            LoadTradeHistory();
            LoadTradingSettings();
            SetupTimer();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            this.Text = "IBKR Trading Pro v3.0";
            this.Size = new Size(900, 600); // Previously 1350x900 or similar
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(700, 500);
            this.WindowState = FormWindowState.Maximized;
            
            // Modern dark theme with professional aesthetics
            this.BackColor = Color.FromArgb(8, 12, 18);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            
            // Enable double buffering for smooth animations
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            
            CreateLayout();
        }

        private void CreateLayout()
        {
            // Only create and add the header once
            if (headerPanel == null)
            {
                headerPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 60,
                    BackColor = Color.FromArgb(15, 20, 30),
                    Padding = new Padding(20, 10, 20, 10)
                };
                this.Controls.Add(headerPanel);

                var titleLabel = new Label
                {
                    Text = "IBKR TRADING PRO",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 200, 255),
                    Location = new Point(20, 10),
                    AutoSize = true
                };
                headerPanel.Controls.Add(titleLabel);

                statusLabel = new Label
                {
                    Text = "● DISCONNECTED",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(255, 100, 100),
                    Location = new Point(300, 20),
                    AutoSize = true
                };
                headerPanel.Controls.Add(statusLabel);

                settingsButton = new Button
                {
                    Text = "SETTINGS",
                    Size = new Size(120, 35),
                    Location = new Point(this.Width - 420, 10),
                    BackColor = Color.FromArgb(60, 80, 100),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                settingsButton.FlatAppearance.BorderSize = 0;
                settingsButton.Click += SettingsButton_Click;
                headerPanel.Controls.Add(settingsButton);

                connectButton = new Button
                {
                    Text = "CONNECT",
                    Size = new Size(120, 35),
                    Location = new Point(this.Width - 290, 10),
                    BackColor = Color.FromArgb(0, 150, 100),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                connectButton.FlatAppearance.BorderSize = 0;
                connectButton.Click += ConnectButton_Click;
                headerPanel.Controls.Add(connectButton);

                disconnectButton = new Button
                {
                    Text = "DISCONNECT",
                    Size = new Size(120, 35),
                    Location = new Point(this.Width - 160, 10),
                    BackColor = Color.FromArgb(200, 50, 50),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Enabled = false
                };
                disconnectButton.FlatAppearance.BorderSize = 0;
                disconnectButton.Click += DisconnectButton_Click;
                headerPanel.Controls.Add(disconnectButton);
            }
            // Main container
            mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(8, 12, 18),
                Padding = new Padding(0)
            };
            this.Controls.Add(mainContainer);

            // Ensure contentPanel is initialized and added to mainContainer
            if (contentPanel == null)
            {
                contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
                mainContainer.Controls.Add(contentPanel);
            }
            CreateContentPanel();
        }

        private void CreateSidebarPanel()
        {
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.FromArgb(12, 18, 28),
                Padding = new Padding(15)
            };
            
            // Add subtle border
            sidebarPanel.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(40, 50, 60), 1))
                {
                    e.Graphics.DrawLine(pen, sidebarPanel.Width - 1, 0, sidebarPanel.Width - 1, sidebarPanel.Height);
                }
            };
            
            mainContainer.Controls.Add(sidebarPanel);
            // Only add navigation/branding here. Remove Trading Rules and other panels.
            // Example branding:
            var brandingLabel = new Label
            {
                Text = "IBKR TRADING PRO",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(15, 20),
                AutoSize = true
            };
            sidebarPanel.Controls.Add(brandingLabel);
            // Add navigation buttons here if needed.
        }

        private void CreateContentPanel()
        {
            // Ensure contentPanel is initialized
            if (contentPanel == null)
            {
                contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
                if (mainContainer != null && !mainContainer.Controls.Contains(contentPanel))
                    mainContainer.Controls.Add(contentPanel);
            }
            contentPanel.Controls.Clear();

            // Ensure accountSummaryPanel is initialized
            if (accountSummaryPanel == null)
            {
                accountSummaryPanel = new Panel
                {
                    BackColor = Color.FromArgb(15, 20, 30)
                };
            }
            // Ensure positionsPanel is initialized
            if (positionsPanel == null)
            {
                positionsPanel = new Panel
                {
                    BackColor = Color.FromArgb(15, 20, 30)
                };
            }
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Trading Rules
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // Trading Panel
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 20F)); // Account Summary
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // Positions
            contentPanel.Controls.Add(table);

            tradingRulesRibbon = new Panel
            {
                BackColor = Color.FromArgb(20, 28, 40)
            };
            // Add rule buttons to tradingRulesRibbon
            var addRuleButton = new Button
            {
                Text = "Add Rule",
                Size = new Size(120, 35),
                Location = new Point(10, 10),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            addRuleButton.FlatAppearance.BorderSize = 0;
            addRuleButton.Click += (s, e) => ShowTradingRulesDialog();
            tradingRulesRibbon.Controls.Add(addRuleButton);

            var manageRulesButton = new Button
            {
                Text = "Manage Rules",
                Size = new Size(120, 35),
                Location = new Point(130, 10),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            manageRulesButton.FlatAppearance.BorderSize = 0;
            manageRulesButton.Click += (s, e) => ShowTradingRulesDialog();
            tradingRulesRibbon.Controls.Add(manageRulesButton);

            table.Controls.Add(tradingRulesRibbon, 0, 0);

            tradingPanel = new Panel
            {
                BackColor = Color.FromArgb(15, 20, 30),
                Dock = DockStyle.Top,
                Height = 250 // Enough to fit all controls
            };
            tradingPanel.Controls.Clear();

            int y = 15;
            var orderEntryLabel = new Label
            {
                Text = "ORDER ENTRY",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(15, y),
                AutoSize = true
            };
            tradingPanel.Controls.Add(orderEntryLabel);
            y += 35;

            // Symbol input
            var symbolLabel = new Label
            {
                Text = "Symbol",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, y),
                AutoSize = true
            };
            tradingPanel.Controls.Add(symbolLabel);

            symbolTextBox = new TextBox
            {
                Text = "AAPL",
                Location = new Point(90, y - 3),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            tradingPanel.Controls.Add(symbolTextBox);

            y += 35;
            // Order type
            var orderTypeLabel = new Label
            {
                Text = "Order Type",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, y),
                AutoSize = true
            };
            tradingPanel.Controls.Add(orderTypeLabel);

            orderTypeComboBox = new ComboBox
            {
                Location = new Point(110, y - 3),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            orderTypeComboBox.Items.AddRange(new string[] { "Market", "Limit", "Stop", "Stop Limit" });
            orderTypeComboBox.SelectedIndex = 0;
            tradingPanel.Controls.Add(orderTypeComboBox);

            y += 35;
            // Quantity
            var quantityLabel = new Label
            {
                Text = "Quantity",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, y),
                AutoSize = true
            };
            tradingPanel.Controls.Add(quantityLabel);

            quantityTextBox = new TextBox
            {
                Text = "100",
                Location = new Point(90, y - 3),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            tradingPanel.Controls.Add(quantityTextBox);

            y += 35;
            // Price
            var priceLabel = new Label
            {
                Text = "Price",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, y),
                AutoSize = true
            };
            tradingPanel.Controls.Add(priceLabel);

            priceTextBox = new TextBox
            {
                Text = "225.48",
                Location = new Point(90, y - 3),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            tradingPanel.Controls.Add(priceTextBox);

            y += 45;
            // Buy/Sell buttons
            buyButton = new Button
            {
                Text = "BUY",
                Size = new Size(100, 40),
                Location = new Point(15, y),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            buyButton.FlatAppearance.BorderSize = 0;
            buyButton.Click += BuyButton_Click;
            tradingPanel.Controls.Add(buyButton);

            sellButton = new Button
            {
                Text = "SELL",
                Size = new Size(100, 40),
                Location = new Point(130, y),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            sellButton.FlatAppearance.BorderSize = 0;
            sellButton.Click += SellButton_Click;
            tradingPanel.Controls.Add(sellButton);

            table.Controls.Add(tradingPanel, 0, 1);

            // Add account summary controls to accountSummaryPanel
            var accountSummaryLabel = new Label
            {
                Text = "ACCOUNT SUMMARY",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(10, 10),
                AutoSize = true
            };
            accountSummaryPanel.Controls.Add(accountSummaryLabel);
            var y2 = 40;
            foreach (var item in new[]
            {
                new { Metric = "Buying Power", Value = "$250,000.00" },
                new { Metric = "Day Trades", Value = "0" },
                new { Metric = "Margin Used", Value = "$0.00" },
                new { Metric = "Cash", Value = "$50,000.00" }
            })
            {
                var label = new Label
                {
                    Text = $"{item.Metric}: {item.Value}",
                    Location = new Point(10, y2),
                    AutoSize = true,
                    ForeColor = Color.White
                };
                accountSummaryPanel.Controls.Add(label);
                y2 += 25;
            }
            table.Controls.Add(accountSummaryPanel, 0, 2);

            // Add positions controls to positionsPanel
            var positionsLabel = new Label
            {
                Text = "POSITIONS",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(10, 10),
                AutoSize = true
            };
            positionsPanel.Controls.Add(positionsLabel);
            var positionsListView = new ListView
            {
                Location = new Point(10, 40),
                Size = new Size(420, 150),
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = Color.FromArgb(20, 28, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None
            };
            positionsListView.Columns.Add("Symbol", 60);
            positionsListView.Columns.Add("Qty", 40);
            positionsListView.Columns.Add("Avg", 50);
            positionsListView.Columns.Add("P&L", 60);
            positionsListView.Columns.Add("%", 60);
            positionsListView.Columns.Add("ATR Multiple", 90);
            foreach (var symbol in new[] { "AAPL", "MSFT", "TSLA" })
            {
                decimal qty = symbol == "AAPL" ? 100 : symbol == "MSFT" ? 50 : 25;
                decimal avgCost = symbol == "AAPL" ? 150 : symbol == "MSFT" ? 300 : 250;
                decimal marketPrice = symbol == "AAPL" ? 155 : symbol == "MSFT" ? 310 : 210;
                decimal pnl = (marketPrice - avgCost) * qty;
                decimal percent = avgCost != 0 ? ((marketPrice - avgCost) / avgCost) * 100 : 0;
                double close = (double)marketPrice;
                double sma50 = 150; // Simulated
                double atr = 2.5; // Simulated
                double atrFactor = (sma50 > 0 && atr > 0) ? close / sma50 / atr : 0;
                var item = new ListViewItem(new[]
                {
                    symbol,
                    qty.ToString(),
                    avgCost.ToString("C2"),
                    pnl.ToString("C2"),
                    percent.ToString("+#.##;-#.##") + "%",
                    atrFactor.ToString("0.00")
                });
                item.BackColor = pnl >= 0 ? Color.FromArgb(0, 100, 0) : Color.FromArgb(120, 0, 0);
                item.ForeColor = Color.White;
                positionsListView.Items.Add(item);
            }
            positionsPanel.Controls.Add(positionsListView);
            table.Controls.Add(positionsPanel, 0, 3);
        }

        private void CreateChartPanel()
        {
            chartPanel = new Panel
            {
                Location = new Point(20, 40), // Moved down to account for header space
                Size = new Size(800, 700), // Expanded to use full available height
                BackColor = Color.FromArgb(15, 20, 30),
                Padding = new Padding(15)
            };
            
            // Add modern border
            chartPanel.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(40, 50, 60), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, chartPanel.Width - 1, chartPanel.Height - 1);
                }
            };
            
            contentPanel.Controls.Add(chartPanel);

            var chartLabel = new Label
            {
                Text = "PRICE CHART",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(15, 15),
                AutoSize = true
            };
            chartPanel.Controls.Add(chartLabel);

            // Simulated chart area
            var chartArea = new Panel
            {
                Location = new Point(15, 50),
                Size = new Size(770, 330),
                BackColor = Color.FromArgb(20, 28, 40)
            };
            
            // Add chart visualization
            chartArea.Paint += (s, e) => {
                // Draw price line
                using (var pen = new Pen(Color.FromArgb(0, 255, 150), 2))
                {
                    var points = new Point[] {
                        new Point(0, 250), new Point(100, 240), new Point(200, 220),
                        new Point(300, 200), new Point(400, 180), new Point(500, 160),
                        new Point(600, 140), new Point(700, 120), new Point(770, 100)
                    };
                    e.Graphics.DrawLines(pen, points);
                }
                
                // Draw grid lines
                using (var pen = new Pen(Color.FromArgb(40, 50, 60), 1))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int y = 50 + i * 40;
                        e.Graphics.DrawLine(pen, 0, y, chartArea.Width, y);
                    }
                }
            };
            
            chartPanel.Controls.Add(chartArea);
        }

        private void CreateOrderPanel()
        {
            orderPanel = new Panel
            {
                Location = new Point(840, 40), // Right side, with header space
                Size = new Size(400, 700), // Same width as before
                BackColor = Color.FromArgb(15, 20, 30),
                Padding = new Padding(15),
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            // Add modern border
            orderPanel.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(40, 50, 60), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, orderPanel.Width - 1, orderPanel.Height - 1);
                }
            };
            
            contentPanel.Controls.Add(orderPanel);

            var orderLabel = new Label
            {
                Text = "ORDER ENTRY",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(15, 15),
                AutoSize = true
            };
            orderPanel.Controls.Add(orderLabel);

            // Symbol input
            var symbolLabel = new Label
            {
                Text = "Symbol",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 220),
                AutoSize = true
            };
            orderPanel.Controls.Add(symbolLabel);

            symbolTextBox = new TextBox
            {
                Text = "AAPL",
                Location = new Point(15, 245),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Add focus effects
            symbolTextBox.Enter += (s, e) => {
                symbolTextBox.BackColor = Color.FromArgb(35, 45, 55);
                symbolTextBox.BorderStyle = BorderStyle.Fixed3D;
            };
            symbolTextBox.Leave += (s, e) => {
                symbolTextBox.BackColor = Color.FromArgb(25, 35, 45);
                symbolTextBox.BorderStyle = BorderStyle.FixedSingle;
            };
            
            orderPanel.Controls.Add(symbolTextBox);

            // Order type
            var orderTypeLabel = new Label
            {
                Text = "Order Type",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(210, 220),
                AutoSize = true
            };
            orderPanel.Controls.Add(orderTypeLabel);

            orderTypeComboBox = new ComboBox
            {
                Location = new Point(210, 245),
                Size = new Size(170, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            orderTypeComboBox.Items.AddRange(new string[] { "Market", "Limit", "Stop", "Stop Limit" });
            orderTypeComboBox.SelectedIndex = 0;
            
            orderTypeComboBox.Enter += (s, e) => {
                orderTypeComboBox.BackColor = Color.FromArgb(35, 45, 55);
            };
            orderTypeComboBox.Leave += (s, e) => {
                orderTypeComboBox.BackColor = Color.FromArgb(25, 35, 45);
            };
            
            orderPanel.Controls.Add(orderTypeComboBox);

            // Quantity
            var quantityLabel = new Label
            {
                Text = "Quantity",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 290),
                AutoSize = true
            };
            orderPanel.Controls.Add(quantityLabel);

            quantityTextBox = new TextBox
            {
                Text = "100",
                Location = new Point(15, 315),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            quantityTextBox.Enter += (s, e) => {
                quantityTextBox.BackColor = Color.FromArgb(35, 45, 55);
                quantityTextBox.BorderStyle = BorderStyle.Fixed3D;
            };
            quantityTextBox.Leave += (s, e) => {
                quantityTextBox.BackColor = Color.FromArgb(25, 35, 45);
                quantityTextBox.BorderStyle = BorderStyle.FixedSingle;
            };
            
            orderPanel.Controls.Add(quantityTextBox);

            // Price
            var priceLabel = new Label
            {
                Text = "Price",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(210, 290),
                AutoSize = true
            };
            orderPanel.Controls.Add(priceLabel);

            priceTextBox = new TextBox
            {
                Text = "225.48",
                Location = new Point(210, 315),
                Size = new Size(170, 30),
                BackColor = Color.FromArgb(25, 35, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            priceTextBox.Enter += (s, e) => {
                priceTextBox.BackColor = Color.FromArgb(35, 45, 55);
                priceTextBox.BorderStyle = BorderStyle.Fixed3D;
            };
            priceTextBox.Leave += (s, e) => {
                priceTextBox.BackColor = Color.FromArgb(25, 35, 45);
                priceTextBox.BorderStyle = BorderStyle.FixedSingle;
            };
            
            orderPanel.Controls.Add(priceTextBox);

            // Buy/Sell buttons
            buyButton = new Button
            {
                Text = "BUY",
                Size = new Size(180, 50),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            buyButton.FlatAppearance.BorderSize = 0;
            buyButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 180, 120);
            buyButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 120, 80);
            
            buyButton.MouseEnter += (s, e) => {
                buyButton.Size = new Size(185, 52);
                buyButton.Location = new Point(13, buyButton.Location.Y - 1);
            };
            buyButton.MouseLeave += (s, e) => {
                buyButton.Size = new Size(180, 50);
                buyButton.Location = new Point(15, buyButton.Location.Y + 1);
            };
            buyButton.Click += BuyButton_Click;
            orderPanel.Controls.Add(buyButton);

            sellButton = new Button
            {
                Text = "SELL",
                Size = new Size(170, 50),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            sellButton.FlatAppearance.BorderSize = 0;
            sellButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 70, 70);
            sellButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 40, 40);
            
            sellButton.MouseEnter += (s, e) => {
                sellButton.Size = new Size(175, 52);
                sellButton.Location = new Point(208, sellButton.Location.Y - 1);
            };
            sellButton.MouseLeave += (s, e) => {
                sellButton.Size = new Size(170, 50);
                sellButton.Location = new Point(210, sellButton.Location.Y + 1);
            };
            sellButton.Click += SellButton_Click;
            orderPanel.Controls.Add(sellButton);
            
            // Set initial button positions at bottom
            buyButton.Location = new Point(15, orderPanel.Height - 80);
            sellButton.Location = new Point(210, orderPanel.Height - 80);
        }



        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            // Use the saved host, port, clientId, and isLiveTrading for connection logic
            // Example: (replace with your actual connection logic)
            string mode = isLiveTrading ? "Live Trading" : "Paper Trading";
            string info = $"Connecting to {host}:{port} (Client ID: {clientId}) - Mode: {mode}";
            MessageBox.Show(info, "Connection Info");
            // TODO: Initialize or update IBKRIntegration/marketDataService with these settings
            // Example:
            // ibkrIntegration.UpdateConnectionSettings(host, port, "TWS", mode);
            // ibkrIntegration.Connect();
            // Or for marketDataService, pass host/port/mode as needed
            await UpdateAccountSummaryAndPositionsAsync();
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            isConnected = false;
            statusLabel.Text = "● DISCONNECTED";
            statusLabel.ForeColor = Color.FromArgb(255, 100, 100);
            connectButton.Enabled = true;
            disconnectButton.Enabled = false;
            
            // Update UI
            UpdateUI();
        }

        private void BuyButton_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Please connect to IBKR first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var symbol = symbolTextBox.Text.Trim().ToUpper();
                var quantity = int.Parse(quantityTextBox.Text);
                var price = decimal.Parse(priceTextBox.Text);
                var orderType = orderTypeComboBox.Text;

                var trade = new Trade
                {
                    Id = tradeHistory.Count + 1,
                    Symbol = symbol,
                    Action = "BUY",
                    Quantity = quantity,
                    Price = price,
                    OrderType = orderType,
                    Status = "FILLED",
                    Timestamp = DateTime.Now,
                    PnL = 0 // Will be calculated later
                };

                tradeHistory.Add(trade);
                SaveTradeHistory();
                UpdateUI();
                
                MessageBox.Show($"BUY order executed: {quantity} {symbol} @ ${price}", "Order Executed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error placing order: {ex.Message}", "Order Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SellButton_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Please connect to IBKR first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var symbol = symbolTextBox.Text.Trim().ToUpper();
                var quantity = int.Parse(quantityTextBox.Text);
                var price = decimal.Parse(priceTextBox.Text);
                var orderType = orderTypeComboBox.Text;

                var trade = new Trade
                {
                    Id = tradeHistory.Count + 1,
                    Symbol = symbol,
                    Action = "SELL",
                    Quantity = quantity,
                    Price = price,
                    OrderType = orderType,
                    Status = "FILLED",
                    Timestamp = DateTime.Now,
                    PnL = 0 // Will be calculated later
                };

                tradeHistory.Add(trade);
                SaveTradeHistory();
                UpdateUI();
                
                MessageBox.Show($"SELL order executed: {quantity} {symbol} @ ${price}", "Order Executed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error placing order: {ex.Message}", "Order Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            LoadTradingSettings(); // Always load latest settings
            using (var dlg = new SettingsDialog(isLiveTrading, host, port, clientId))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    isLiveTrading = dlg.IsLiveTrading;
                    host = dlg.Host;
                    port = dlg.Port;
                    clientId = dlg.ClientId;
                    SaveTradingSettings();
                    // TODO: Reconnect to IBKR with new mode/host/port/clientId
                }
            }
        }

        private void SetupTimer()
        {
            updateTimer = new Timer
            {
                Interval = 1000 // Update every second
            };
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (isConnected)
            {
                await UpdateMarketData();
            }
        }

        private async Task UpdateMarketData()
        {
            await UpdateAccountSummaryAndPositionsAsync();
            // Simulate price changes for positions
            var random = new Random();
            
            // Update positions with simulated P&L changes
            if (positionsListView != null && positionsListView.Items.Count > 0)
            {
                for (int i = 0; i < positionsListView.Items.Count; i++)
                {
                    var item = positionsListView.Items[i];
                    var currentPnL = decimal.Parse(item.SubItems[3].Text.Replace("$", "").Replace(",", ""));
                    var change = (decimal)(random.NextDouble() - 0.5) * 50; // Random P&L change
                    var newPnL = currentPnL + change;
                    
                    item.SubItems[3].Text = newPnL >= 0 ? $"+${newPnL:F2}" : $"-${Math.Abs(newPnL):F2}";
                    item.SubItems[4].Text = newPnL >= 0 ? $"+{(newPnL/1000*100):F2}%" : $"-{(Math.Abs(newPnL)/1000*100):F2}%";
                    
                    // Color coding
                    if (newPnL >= 0)
                    {
                        item.SubItems[3].ForeColor = Color.FromArgb(0, 255, 150);
                        item.SubItems[4].ForeColor = Color.FromArgb(0, 255, 150);
                    }
                    else
                    {
                        item.SubItems[3].ForeColor = Color.FromArgb(255, 100, 100);
                        item.SubItems[4].ForeColor = Color.FromArgb(255, 100, 100);
                    }
                }
            }
        }

        private async Task UpdateAccountSummaryAndPositionsAsync()
        {
            // Example simulated data
            var accountSummary = new[]
            {
                new { Metric = "Buying Power", Value = "$250,000.00" },
                new { Metric = "Day Trades", Value = "0" },
                new { Metric = "Margin Used", Value = "$0.00" },
                new { Metric = "Cash", Value = "$50,000.00" }
            };
            accountSummaryPanel.Controls.Clear();
            var accountSummaryLabel = new Label
            {
                Text = "ACCOUNT SUMMARY",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(10, 10),
                AutoSize = true
            };
            accountSummaryPanel.Controls.Add(accountSummaryLabel);
            var y = 40;
            foreach (var item in accountSummary)
            {
                var label = new Label
                {
                    Text = $"{item.Metric}: {item.Value}",
                    Location = new Point(10, y),
                    AutoSize = true,
                    ForeColor = Color.White
                };
                accountSummaryPanel.Controls.Add(label);
                y += 25;
            }

            // Simulated positions data
            var symbols = new[] { "AAPL", "MSFT", "TSLA" };
            positionsPanel.Controls.Clear();
            var positionsLabel = new Label
            {
                Text = "POSITIONS",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                Location = new Point(10, 10),
                AutoSize = true
            };
            positionsPanel.Controls.Add(positionsLabel);
            var positionsListView = new ListView
            {
                Location = new Point(10, 40),
                Size = new Size(420, 150),
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = Color.FromArgb(20, 28, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None
            };
            positionsListView.Columns.Add("Symbol", 60);
            positionsListView.Columns.Add("Qty", 40);
            positionsListView.Columns.Add("Avg", 50);
            positionsListView.Columns.Add("P&L", 60);
            positionsListView.Columns.Add("%", 60);
            positionsListView.Columns.Add("ATR Multiple", 90);
            foreach (var symbol in new[] { "AAPL", "MSFT", "TSLA" })
            {
                decimal qty = symbol == "AAPL" ? 100 : symbol == "MSFT" ? 50 : 25;
                decimal avgCost = symbol == "AAPL" ? 150 : symbol == "MSFT" ? 300 : 250;
                decimal marketPrice = symbol == "AAPL" ? 155 : symbol == "MSFT" ? 310 : 210;
                decimal pnl = (marketPrice - avgCost) * qty;
                decimal percent = avgCost != 0 ? ((marketPrice - avgCost) / avgCost) * 100 : 0;
                double close = (double)marketPrice;
                double sma50 = 150; // Simulated
                double atr = 2.5; // Simulated
                double atrFactor = (sma50 > 0 && atr > 0) ? close / sma50 / atr : 0;
                var item = new ListViewItem(new[]
                {
                    symbol,
                    qty.ToString(),
                    avgCost.ToString("C2"),
                    pnl.ToString("C2"),
                    percent.ToString("+#.##;-#.##") + "%",
                    atrFactor.ToString("0.00")
                });
                item.BackColor = pnl >= 0 ? Color.FromArgb(0, 100, 0) : Color.FromArgb(120, 0, 0);
                item.ForeColor = Color.White;
                positionsListView.Items.Add(item);
            }
            positionsPanel.Controls.Add(positionsListView);
        }

        private void UpdateUI()
        {
            try
            {
                // Add null checks for all controls
                if (positionsListView == null) return;
                // Add similar checks for other controls as needed
                // Update account summary
                var totalPnL = tradeHistory.Sum(t => t.PnL);
                pnlLabel.Text = totalPnL >= 0 ? $"+${totalPnL:F2}" : $"-${Math.Abs(totalPnL):F2}";
                pnlLabel.ForeColor = totalPnL >= 0 ? Color.FromArgb(0, 255, 150) : Color.FromArgb(255, 100, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception in UpdateUI: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void LoadTradeHistory()
        {
            try
            {
                if (File.Exists("trades_history.json"))
                {
                    var json = File.ReadAllText("trades_history.json");
                    tradeHistory = JsonSerializer.Deserialize<List<Trade>>(json) ?? new List<Trade>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception in LoadTradeHistory: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void SaveTradeHistory()
        {
            try
            {
                var json = JsonSerializer.Serialize(tradeHistory, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("trades_history.json", json);
            }
            catch (Exception ex)
            {
                // Handle error silently
            }
        }

        private void LoadTradingSettings()
        {
            try
            {
                if (File.Exists("trading_settings.json"))
                {
                    var json = File.ReadAllText("trading_settings.json");
                    var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (settings != null)
                    {
                        if (settings.TryGetValue("TradingMode", out var mode))
                            isLiveTrading = mode.ToString() == "Live Trading";
                        if (settings.TryGetValue("Host", out var h))
                            host = h.ToString() ?? "127.0.0.1";
                        if (settings.TryGetValue("Port", out var p) && int.TryParse(p.ToString(), out int portVal))
                            port = portVal;
                        if (settings.TryGetValue("ClientId", out var c) && int.TryParse(c.ToString(), out int clientIdVal))
                            clientId = clientIdVal;
                    }
                }
            }
            catch { isLiveTrading = false; host = "127.0.0.1"; port = 7497; clientId = 11; }
        }

        private void SaveTradingSettings()
        {
            var settings = new Dictionary<string, object>
            {
                ["TradingMode"] = isLiveTrading ? "Live Trading" : "Paper Trading",
                ["Host"] = host,
                ["Port"] = port,
                ["ClientId"] = clientId
            };
            File.WriteAllText("trading_settings.json", JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        }

        private void ShowTradingRulesDialog()
        {
            var rulesForm = new Form
            {
                Text = "Trading Rules Management",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(8, 12, 18),
                ForeColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var rulesListView = new ListView
            {
                Location = new Point(20, 20),
                Size = new Size(560, 350),
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = Color.FromArgb(20, 28, 40),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.None
            };
            
            rulesListView.Columns.Add("Rule Name", 150);
            rulesListView.Columns.Add("Stop Loss", 120);
            rulesListView.Columns.Add("Profit Taking", 120);
            rulesListView.Columns.Add("Trailing Stop", 100);
            rulesListView.Columns.Add("Selected", 80);

            // Load and display rules
            var rules = TradingRules.LoadRules();
            foreach (var rule in rules)
            {
                var item = new ListViewItem(new string[] { 
                    rule.Name, 
                    rule.StopLoss.Length > 15 ? rule.StopLoss.Substring(0, 15) + "..." : rule.StopLoss,
                    rule.ProfitTaking.Length > 15 ? rule.ProfitTaking.Substring(0, 15) + "..." : rule.ProfitTaking,
                    rule.TrailingStop.Length > 15 ? rule.TrailingStop.Substring(0, 15) + "..." : rule.TrailingStop,
                    rule.Selected ? "●" : "○"
                });
                
                if (rule.Selected)
                {
                    item.ForeColor = Color.FromArgb(0, 255, 150);
                }
                
                rulesListView.Items.Add(item);
            }

            rulesForm.Controls.Add(rulesListView);

            // Add buttons
            var selectButton = new Button
            {
                Text = "Select Rule",
                Size = new Size(120, 35),
                Location = new Point(20, 390),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            selectButton.FlatAppearance.BorderSize = 0;
            selectButton.Click += (s, e) => {
                if (rulesListView.SelectedItems.Count > 0)
                {
                    var selectedIndex = rulesListView.SelectedIndices[0];
                    // Update selection logic here
                    rulesForm.Close();
                }
            };
            rulesForm.Controls.Add(selectButton);

            var closeButton = new Button
            {
                Text = "Close",
                Size = new Size(120, 35),
                Location = new Point(460, 390),
                BackColor = Color.FromArgb(60, 80, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => rulesForm.Close();
            rulesForm.Controls.Add(closeButton);

            rulesForm.ShowDialog(this);
        }
    }

    public class Trade
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = "";
        public string Action { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string OrderType { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public decimal PnL { get; set; }
    }

    public class SettingsDialog : Form
    {
        private TabControl tabControl;
        private TabPage tradingTab;
        private CheckBox liveTradingToggle;
        private TextBox hostTextBox;
        private TextBox portTextBox;
        private TextBox clientIdTextBox;
        private Button okButton;
        public bool IsLiveTrading { get; private set; }
        public string Host { get; private set; } = "127.0.0.1";
        public int Port { get; private set; } = 7497;
        public int ClientId { get; private set; } = 11;

        public SettingsDialog(bool isLiveTrading, string host, int port, int clientId)
        {
            this.Text = "Settings";
            this.Size = new Size(400, 300);
            this.MinimumSize = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(8, 12, 18);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = false
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // TabControl
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // OK button
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Appearance = TabAppearance.Normal
            };
            tabControl.ItemSize = new Size(120, 40);
            tabControl.SizeMode = TabSizeMode.Fixed;

            tradingTab = new TabPage("Trading")
            {
                BackColor = Color.FromArgb(20, 28, 40),
                ForeColor = Color.White
            };
            tradingTab.MinimumSize = new Size(350, 200);

            liveTradingToggle = new CheckBox
            {
                Text = "Enable Live Trading (uncheck for Paper Trading)",
                Checked = isLiveTrading,
                Location = new Point(30, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            tradingTab.Controls.Add(liveTradingToggle);

            var fieldsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 10, 20, 10),
                AutoSize = false,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            fieldsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Live trading toggle
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Host
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Port
            fieldsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // ClientId

            liveTradingToggle.Dock = DockStyle.Left;
            fieldsLayout.Controls.Add(liveTradingToggle, 0, 0);
            fieldsLayout.SetColumnSpan(liveTradingToggle, 2);

            var hostLabel = new Label { Text = "Host:", Anchor = AnchorStyles.Left, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            hostTextBox = new TextBox { Text = host, Dock = DockStyle.Fill };
            fieldsLayout.Controls.Add(hostLabel, 0, 1);
            fieldsLayout.Controls.Add(hostTextBox, 1, 1);

            var portLabel = new Label { Text = "Port:", Anchor = AnchorStyles.Left, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            portTextBox = new TextBox { Text = port.ToString(), Dock = DockStyle.Fill };
            fieldsLayout.Controls.Add(portLabel, 0, 2);
            fieldsLayout.Controls.Add(portTextBox, 1, 2);

            var clientIdLabel = new Label { Text = "Client ID:", Anchor = AnchorStyles.Left, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft };
            clientIdTextBox = new TextBox { Text = clientId.ToString(), Dock = DockStyle.Fill };
            fieldsLayout.Controls.Add(clientIdLabel, 0, 3);
            fieldsLayout.Controls.Add(clientIdTextBox, 1, 3);

            tradingTab.Controls.Clear();
            tradingTab.Controls.Add(fieldsLayout);
            tradingTab.Dock = DockStyle.Fill;

            tabControl.TabPages.Add(tradingTab);
            layout.Controls.Add(tabControl, 0, 0);

            okButton = new Button
            {
                Text = "OK",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Click += (s, e) => {
                IsLiveTrading = liveTradingToggle.Checked;
                Host = hostTextBox.Text.Trim();
                int.TryParse(portTextBox.Text.Trim(), out int portVal);
                Port = portVal > 0 ? portVal : 7497;
                int.TryParse(clientIdTextBox.Text.Trim(), out int clientIdVal);
                ClientId = clientIdVal > 0 ? clientIdVal : 11;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            var okPanel = new Panel { Dock = DockStyle.Fill, Height = 50, BackColor = Color.Transparent };
            okButton.Location = new Point(okPanel.Width - okButton.Width - 20, 7);
            okButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            okPanel.Controls.Add(okButton);
            layout.Controls.Add(okPanel, 0, 1);

            this.Controls.Add(layout);
        }
    }
} 