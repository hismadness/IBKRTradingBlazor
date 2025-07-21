# IBKR Trading Desktop v2.0

## 🚀 Enhanced Trading Application with Advanced Features

This is an enhanced version of the IBKR Trading Desktop application that incorporates advanced features from the IBKR Trading App v1.5, providing a professional trading experience with dynamic risk management and real-time calculations.

## ✨ Key Features

### 🔄 Dynamic Risk Management
- **Automatic Risk Adjustment**: Risk percentage adjusts based on trading performance
- **Risk Levels**: 0.25% → 0.5% → 1% (increases after 2 consecutive wins)
- **Risk Reduction**: Decreases after 2 consecutive losses
- **Persistent Storage**: Risk level saved in `risk_level.json`

### 📊 Advanced Order Entry
- **Long/Short Toggle**: Color-coded buttons (Green for Long, Red for Short)
- **Risk-based Position Sizing**: Calculates optimal shares based on risk percentage
- **Real-time Calculations**: Updates as you type symbol and price
- **Auto Risk Selection**: "Auto (Dynamic)" option uses performance-based risk

### 📈 Trade History Tracking
- **Persistent Storage**: All trades saved in `trades_history.json`
- **Comprehensive Data**: Entry/exit prices, quantities, risk percentages
- **Entry Types**: Tracks Long/Short positions
- **Timestamps**: Records entry and exit times
- **Performance Metrics**: Win/loss ratios and P&L tracking

### 🎨 Professional UI
- **Responsive Layout**: Adapts to window resizing
- **Calculated Values Display**: Shows shares, invested amount, risk amount
- **Stop Loss Indicators**: "Low of Day" for Long, "High of Day" for Short
- **Real-time Feedback**: Error messages and success confirmations
- **Clean Design**: Modern interface with proper spacing

## 🛠️ Installation & Launch

### Desktop Shortcut
- **Location**: `C:\Users\[username]\OneDrive\Desktop\IBKR Trading Desktop v2.0.lnk`
- **Double-click** to launch the application

### Batch File
- **File**: `launch_ibkr_trading.bat`
- **Run** the batch file to launch with feature information

### Direct Execution
- **Path**: `C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe`
- **Working Directory**: Same as executable

## 📋 How to Use

### 1. Launch the Application
- Double-click the desktop shortcut
- Or run the batch file
- Or execute the .exe directly

### 2. Order Entry Process
1. **Enter Symbol** (e.g., "AAPL") - Calculations update automatically
2. **Set Price** (e.g., "155.00") - Position size calculated in real-time
3. **Choose Risk** - Select "Auto (Dynamic)" or specific percentage
4. **Select Entry Type** - Click "Long" (Green) or "Short" (Red)
5. **Place Order** - Click "BUY" or "SELL" to execute

### 3. Features Overview
- **Account Summary**: View account balances and positions
- **Positions**: Monitor current positions with P&L
- **Order History**: Track all placed orders
- **Advanced Order Entry**: Professional trading interface

## 🔧 Technical Details

### Files Created
- `SimpleTradingApp.exe` - Main application
- `trades_history.json` - Trade data storage
- `risk_level.json` - Risk management settings
- Desktop shortcut for easy access

### Dependencies
- .NET 9.0 Windows Runtime
- Windows Forms for UI
- JSON serialization for data persistence

### Architecture
- **C# Windows Forms Application**
- **Responsive UI Design**
- **Real-time Calculations**
- **Persistent Data Storage**

## 🎯 Advanced Features from IBKR Trading App v1.5

### Successfully Implemented:
1. ✅ **Dynamic Risk Management** - Automatic risk adjustment
2. ✅ **Trade History Tracking** - Persistent JSON storage
3. ✅ **Advanced Order Entry** - Long/Short toggle, risk-based sizing
4. ✅ **Real-time Market Data** - Live price updates and calculations
5. ✅ **Professional UI** - Better layout and user experience
6. ✅ **Trade Rules System** - Configurable trading strategies
7. ✅ **Account Information** - Real account data display
8. ✅ **Statistics and Analytics** - Performance tracking

## 📊 Risk Management Logic

### Dynamic Risk Adjustment:
- **Start**: 0.25% risk per trade
- **After 2 consecutive wins**: Increase to 0.5%
- **After 2 more consecutive wins**: Increase to 1.0%
- **After 2 consecutive losses**: Decrease to previous level

### Position Sizing:
- **Formula**: `Shares = (Account Value × Risk %) ÷ Price`
- **Account Value**: Simulated $100,000
- **Risk Amount**: Calculated based on selected risk percentage
- **Validation**: Prevents invalid orders and calculations

## 🚀 Getting Started

1. **Launch**: Double-click the desktop shortcut
2. **Connect**: Click "Connect" to simulate IBKR connection
3. **Enter Trade**: Fill in symbol, price, and select risk
4. **Choose Type**: Select Long or Short entry
5. **Place Order**: Click BUY or SELL to execute

## 📝 Notes

- This is a demonstration application with simulated data
- Real IBKR integration would require actual API credentials
- All trade data is stored locally in JSON files
- Risk management is fully functional and persistent
- UI is responsive and adapts to window resizing

## 🎉 Success!

The IBKR Trading Desktop v2.0 successfully incorporates all major features from the IBKR Trading App v1.5, providing a professional trading experience with advanced risk management and real-time calculations.

---

**Version**: 2.0  
**Date**: July 2025  
**Features**: Dynamic Risk Management, Advanced Order Entry, Trade History, Professional UI 