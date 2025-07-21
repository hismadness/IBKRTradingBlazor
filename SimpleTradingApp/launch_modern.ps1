# Modern IBKR Trading Desktop Launcher
# This script launches the SimpleTradingApp with modern UI enhancements

Write-Host "🚀 IBKR Trading Desktop v2.0 - Modern UI" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
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
Write-Host "🎨 Modern UI Features:" -ForegroundColor Cyan
Write-Host "   • Dark theme with Material Design colors" -ForegroundColor White
Write-Host "   • Professional trading aesthetics" -ForegroundColor White
Write-Host "   • Enhanced button styling with hover effects" -ForegroundColor White
Write-Host "   • Improved typography with Segoe UI font" -ForegroundColor White
Write-Host "   • Better contrast and readability" -ForegroundColor White
Write-Host ""

# Start the application
Write-Host "🚀 Launching IBKR Trading Desktop..." -ForegroundColor Green
try {
    Start-Process -FilePath $exePath -WindowStyle Normal
    Write-Host "✅ Application started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 Features Available:" -ForegroundColor Cyan
    Write-Host "   • Real-time market data integration" -ForegroundColor White
    Write-Host "   • Advanced formula evaluation engine" -ForegroundColor White
    Write-Host "   • Dynamic position sizing" -ForegroundColor White
    Write-Host "   • Risk management with trade history" -ForegroundColor White
    Write-Host "   • Session-aware order types" -ForegroundColor White
    Write-Host "   • Professional trading interface" -ForegroundColor White
    Write-Host ""
    Write-Host "💡 Tip: The application now features a modern dark theme" -ForegroundColor Yellow
    Write-Host "💡 Tip: All controls have been updated with Material Design colors" -ForegroundColor Yellow
    Write-Host "💡 Tip: Enhanced user experience with hover effects and better typography" -ForegroundColor Yellow
}
catch {
    Write-Host "❌ Failed to start application: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🎯 Ready for professional trading!" -ForegroundColor Green 