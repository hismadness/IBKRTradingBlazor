# IBKR Trading Desktop v2.0 - 2025 Enhanced UI Launcher
# This script launches the SimpleTradingApp with 2025-level UI enhancements

Write-Host "🚀 IBKR Trading Desktop v2.0 - 2025 Enhanced UI" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan
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
Write-Host "🎨 2025-Level UI Features Implemented:" -ForegroundColor Magenta
Write-Host "   • Micro-Interactions 2.0 with adaptive animations" -ForegroundColor White
Write-Host "   • Button hover effects with size and position changes" -ForegroundColor White
Write-Host "   • Gradient headers with shadow effects" -ForegroundColor White
Write-Host "   • Layered panels with subtle borders" -ForegroundColor White
Write-Host "   • Enhanced focus effects on input controls" -ForegroundColor White
Write-Host "   • Modern dark theme with deeper contrast" -ForegroundColor White
Write-Host "   • Smooth double-buffered rendering" -ForegroundColor White
Write-Host "   • Professional Material Design color palette" -ForegroundColor White
Write-Host ""

# Start the application
Write-Host "🚀 Launching IBKR Trading Desktop with 2025 UI..." -ForegroundColor Green
try {
    Start-Process -FilePath $exePath -WindowStyle Normal
    Write-Host "✅ Application started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎯 2025 UI Enhancements Active:" -ForegroundColor Cyan
    Write-Host "   • Micro-interactions: Buttons expand on hover" -ForegroundColor White
    Write-Host "   • Visual feedback: Enhanced focus states" -ForegroundColor White
    Write-Host "   • Gradient effects: Professional header styling" -ForegroundColor White
    Write-Host "   • Layered design: Depth and dimensionality" -ForegroundColor White
    Write-Host "   • Smooth animations: Double-buffered rendering" -ForegroundColor White
    Write-Host "   • Modern aesthetics: 2025 design trends" -ForegroundColor White
    Write-Host ""
    Write-Host "💡 Interactive Features:" -ForegroundColor Yellow
    Write-Host "   • Hover over buttons to see micro-interactions" -ForegroundColor White
    Write-Host "   • Click on input fields for focus effects" -ForegroundColor White
    Write-Host "   • Notice the gradient headers and layered panels" -ForegroundColor White
    Write-Host "   • Experience smooth, professional animations" -ForegroundColor White
    Write-Host ""
    Write-Host "🎨 Design Philosophy:" -ForegroundColor Magenta
    Write-Host "   • Minimalist Maximalism: Clean layouts with rich effects" -ForegroundColor White
    Write-Host "   • Context-aware interactions: Adaptive micro-animations" -ForegroundColor White
    Write-Host "   • Professional aesthetics: Trading-focused design" -ForegroundColor White
    Write-Host "   • Modern accessibility: Enhanced contrast and focus" -ForegroundColor White
}
catch {
    Write-Host "❌ Failed to start application: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🎯 Ready for professional trading with 2025-level UI!" -ForegroundColor Green 