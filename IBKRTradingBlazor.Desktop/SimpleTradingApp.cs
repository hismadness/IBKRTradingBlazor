using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IBKRTradingBlazor.Desktop
{
    public partial class SimpleTradingApp : Form
    {
        private bool isConnected = false;
        private ListView positionsListView;
        private ListView accountListView;
        private ListView ordersListView;
        private Button connectButton;
        private Button disconnectButton;
        private Label statusLabel;

        public SimpleTradingApp()
        {
            InitializeComponent();
            InitializeTradingInterface();
        }

        private void InitializeComponent()
        {
            this.Text = "IBKR Trading Desktop";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeTradingInterface()
        {
            // Status Panel
            var statusPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.LightGray
            };

            statusLabel = new Label
            {
                Text = "Status: Not Connected",
                Location = new Point(10, 20),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            connectButton = new Button
            {
                Text = "Connect to IBKR",
                Location = new Point(220, 15),
                Size = new Size(120, 30),
                BackColor = Color.Green,
                ForeColor = Color.White
            };
            connectButton.Click += ConnectButton_Click;

            disconnectButton = new Button
            {
                Text = "Disconnect",
                Location = new Point(350, 15),
                Size = new Size(120, 30),
                BackColor = Color.Red,
                ForeColor = Color.White,
                Enabled = false
            };
            disconnectButton.Click += DisconnectButton_Click;

            statusPanel.Controls.Add(statusLabel);
            statusPanel.Controls.Add(connectButton);
            statusPanel.Controls.Add(disconnectButton);

            // Main Container
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Account Summary Panel
            var accountPanel = CreateAccountPanel();
            mainContainer.Controls.Add(accountPanel, 0, 0);

            // Positions Panel
            var positionsPanel = CreatePositionsPanel();
            mainContainer.Controls.Add(positionsPanel, 1, 0);

            // Orders Panel
            var ordersPanel = CreateOrdersPanel();
            mainContainer.Controls.Add(ordersPanel, 0, 1);
            mainContainer.SetColumnSpan(ordersPanel, 2);

            this.Controls.Add(statusPanel);
            this.Controls.Add(mainContainer);
        }

        private Panel CreateAccountPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = "Account Summary",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            accountListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            accountListView.Columns.Add("Item", 150);
            accountListView.Columns.Add("Value", 150);
            accountListView.Columns.Add("Currency", 100);

            // Add sample data
            accountListView.Items.Add(new ListViewItem(new[] { "Net Liquidation", "$100,000.00", "USD" }));
            accountListView.Items.Add(new ListViewItem(new[] { "Total Cash Value", "$50,000.00", "USD" }));
            accountListView.Items.Add(new ListViewItem(new[] { "Buying Power", "$200,000.00", "USD" }));
            accountListView.Items.Add(new ListViewItem(new[] { "Available Funds", "$50,000.00", "USD" }));

            panel.Controls.Add(label);
            panel.Controls.Add(accountListView);

            return panel;
        }

        private Panel CreatePositionsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = "Positions",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            positionsListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            positionsListView.Columns.Add("Symbol", 80);
            positionsListView.Columns.Add("Position", 80);
            positionsListView.Columns.Add("Avg Cost", 100);
            positionsListView.Columns.Add("Market Price", 100);
            positionsListView.Columns.Add("P&L %", 80);

            // Add sample data
            var aaplItem = new ListViewItem(new[] { "AAPL", "100", "$150.00", "$155.00", "+3.33%" });
            aaplItem.SubItems[4].ForeColor = Color.Green;
            positionsListView.Items.Add(aaplItem);

            var msftItem = new ListViewItem(new[] { "MSFT", "50", "$300.00", "$310.00", "+3.33%" });
            msftItem.SubItems[4].ForeColor = Color.Green;
            positionsListView.Items.Add(msftItem);

            panel.Controls.Add(label);
            panel.Controls.Add(positionsListView);

            return panel;
        }

        private Panel CreateOrdersPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Height = 200
            };

            var label = new Label
            {
                Text = "Order History",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            ordersListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            ordersListView.Columns.Add("Order ID", 80);
            ordersListView.Columns.Add("Symbol", 80);
            ordersListView.Columns.Add("Side", 60);
            ordersListView.Columns.Add("Shares", 80);
            ordersListView.Columns.Add("Price", 100);
            ordersListView.Columns.Add("Time", 150);
            ordersListView.Columns.Add("P&L", 100);

            // Add sample data
            var buyItem = new ListViewItem(new[] { "1", "AAPL", "BUY", "100", "$150.00", "2024-01-15 09:30:00", "$500.00" });
            buyItem.SubItems[6].ForeColor = Color.Green;
            ordersListView.Items.Add(buyItem);

            var sellItem = new ListViewItem(new[] { "2", "MSFT", "SELL", "25", "$305.00", "2024-01-14 14:45:00", "-$50.00" });
            sellItem.SubItems[6].ForeColor = Color.Red;
            ordersListView.Items.Add(sellItem);

            panel.Controls.Add(label);
            panel.Controls.Add(ordersListView);

            return panel;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            isConnected = true;
            statusLabel.Text = "Status: Connected to IBKR";
            statusLabel.ForeColor = Color.Green;
            connectButton.Enabled = false;
            disconnectButton.Enabled = true;
            MessageBox.Show("Connected to IBKR successfully!", "Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            isConnected = false;
            statusLabel.Text = "Status: Not Connected";
            statusLabel.ForeColor = Color.Red;
            connectButton.Enabled = true;
            disconnectButton.Enabled = false;
            MessageBox.Show("Disconnected from IBKR", "Disconnection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimpleTradingApp());
        }
    }
} 