# Clear Trade History Script
# This script removes all trade history files and resets the application state

Write-Host "🧹 Clearing Trade History..." -ForegroundColor Yellow

# Remove trade history files if they exist
$filesToRemove = @(
    "trades_history.json",
    "risk_level.json"
)

foreach ($file in $filesToRemove) {
    if (Test-Path $file) {
        Remove-Item $file -Force
        Write-Host "✅ Removed: $file" -ForegroundColor Green
    } else {
        Write-Host "ℹ️  File not found: $file" -ForegroundColor Cyan
    }
}

# Kill any running instances of the application
$processes = Get-Process -Name "SimpleTradingApp" -ErrorAction SilentlyContinue
if ($processes) {
    foreach ($process in $processes) {
        $process.Kill()
        Write-Host "🔄 Killed process: $($process.ProcessName) (PID: $($process.Id))" -ForegroundColor Yellow
    }
} else {
    Write-Host "ℹ️  No SimpleTradingApp processes found" -ForegroundColor Cyan
}

Write-Host "✅ Trade history cleared successfully!" -ForegroundColor Green
Write-Host "🚀 You can now restart the application with a clean state." -ForegroundColor Green 