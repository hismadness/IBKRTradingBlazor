@echo off
echo Starting IBKR Trading Application (Enhanced Version)...
cd /d "%~dp0"
start "" "bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe"
echo Enhanced application started with new features!
echo - 3-column layout with Order Entry panel
echo - Real-time price updates
echo - BUY/SELL order functionality
echo - Improved account summary
pause 