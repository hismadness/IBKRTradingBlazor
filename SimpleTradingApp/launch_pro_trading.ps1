# IBKR Trading Pro v3.0 - Modern Professional Trading Interface
# Inspired by professional trading platforms with 2025 design trends

Write-Host "🚀 IBKR Trading Pro v3.0 - Professional Trading Interface" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Kill any existing instances
$existingProcesses = Get-Process -Name "SimpleTradingApp" -ErrorAction SilentlyContinue
if ($existingProcesses) {
    Write-Host "🔄 Closing existing instances..." -ForegroundColor Yellow
    foreach ($process in $existingProcesses) {
        $process.Kill()
        Start-Sleep -Milliseconds 500
    }
    Write-Host "✅ Existing instances closed" -ForegroundColor Green
}

# Check if executable exists
$exePath = ".\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "❌ Error: Executable not found at $exePath" -ForegroundColor Red
    Write-Host "💡 Please build the project first: dotnet build --configuration Release" -ForegroundColor Yellow
    Write-Host "💡 Then publish: dotnet publish --configuration Release --self-contained --runtime win-x64" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Executable found" -ForegroundColor Green
Write-Host ""
Write-Host "🎨 Professional Trading Interface Features:" -ForegroundColor Magenta
Write-Host "   • Modern Dark Theme with Gradient Backgrounds" -ForegroundColor White
Write-Host "   • Professional Layout with Sidebar Navigation" -ForegroundColor White
Write-Host "   • Real-time Price Chart Visualization" -ForegroundColor White
Write-Host "   • Advanced Order Entry Panel" -ForegroundColor White
Write-Host "   • Live Watchlist with Price Updates" -ForegroundColor White
Write-Host "   • Account Summary with P&L Tracking" -ForegroundColor White
Write-Host "   • Positions Management Dashboard" -ForegroundColor White
Write-Host "   • Micro-interactions and Smooth Animations" -ForegroundColor White
Write-Host ""

# Start the application
Write-Host "🚀 Launching IBKR Trading Pro v3.0..." -ForegroundColor Green
try {
    Start-Process -FilePath $exePath -WindowStyle Normal
    Write-Host "✅ Application started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎯 Professional Trading Features:" -ForegroundColor Cyan
    Write-Host "   • Full-screen trading interface" -ForegroundColor White
    Write-Host "   • Real-time market data simulation" -ForegroundColor White
    Write-Host "   • Advanced order types (Market, Limit, Stop)" -ForegroundColor White
    Write-Host "   • Professional color coding (Green/Red)" -ForegroundColor White
    Write-Host "   • Responsive layout with modern aesthetics" -ForegroundColor White
    Write-Host "   • Account balance and P&L tracking" -ForegroundColor White
    Write-Host ""
    Write-Host "💡 How to Use:" -ForegroundColor Yellow
    Write-Host "   • Click 'CONNECT' to start trading" -ForegroundColor White
    Write-Host "   • Enter symbol, quantity, and price" -ForegroundColor White
    Write-Host "   • Choose order type from dropdown" -ForegroundColor White
    Write-Host "   • Click BUY/SELL to execute orders" -ForegroundColor White
    Write-Host "   • Monitor positions and watchlist" -ForegroundColor White
    Write-Host "   • View real-time price chart" -ForegroundColor White
    Write-Host ""
    Write-Host "🎨 Design Inspiration:" -ForegroundColor Magenta
    Write-Host "   • Professional trading platform aesthetics" -ForegroundColor White
    Write-Host "   • Modern dark theme with blue accents" -ForegroundColor White
    Write-Host "   • Clean, organized layout" -ForegroundColor White
    Write-Host "   • Intuitive navigation and controls" -ForegroundColor White
    Write-Host "   • Real-time data visualization" -ForegroundColor White
}
catch {
    Write-Host "❌ Failed to start application: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🎯 Ready for professional trading!" -ForegroundColor Green
Write-Host "📊 Monitor your positions and execute trades with confidence!" -ForegroundColor Green 