@echo off
echo Starting IBKR Trading Desktop...
cd /d "%~dp0"
cd "bin\Release\net9.0-windows10.0.19041.0\win10-x64"
start "" "IBKRTradingBlazor.Desktop.exe"
echo Application started!
pause 