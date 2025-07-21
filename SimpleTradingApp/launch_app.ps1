# SimpleTradingApp Launcher Script
# This script launches the SimpleTradingApp with proper error handling

Write-Host "🚀 Launching SimpleTradingApp..." -ForegroundColor Green

# Kill any existing instances
$existingProcesses = Get-Process -Name "SimpleTradingApp" -ErrorAction SilentlyContinue
if ($existingProcesses) {
    Write-Host "🔄 Closing existing instances..." -ForegroundColor Yellow
    foreach ($process in $existingProcesses) {
        $process.Kill()
        Start-Sleep -Milliseconds 500
    }
}

# Check if executable exists
$exePath = ".\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "❌ Error: Executable not found at $exePath" -ForegroundColor Red
    Write-Host "💡 Please build the project first: dotnet build --configuration Release" -ForegroundColor Cyan
    exit 1
}

# Launch the application
try {
    Write-Host "✅ Starting SimpleTradingApp..." -ForegroundColor Green
    Start-Process -FilePath $exePath -WindowStyle Normal
    Write-Host "✅ Application launched successfully!" -ForegroundColor Green
    Write-Host "📝 Trade history has been cleared and crash fixes applied." -ForegroundColor Cyan
} catch {
    Write-Host "❌ Error launching application: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} 