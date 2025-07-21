$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$env:USERPROFILE\Desktop\IBKR Trading App (Enhanced).lnk")
$Shortcut.TargetPath = "C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe"
$Shortcut.WorkingDirectory = "C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\"
$Shortcut.Description = "IBKR Trading Desktop Application (Enhanced Version)"
$Shortcut.Save()

Write-Host "Enhanced desktop shortcut created successfully!" -ForegroundColor Green
Write-Host "You can now double-click the 'IBKR Trading App (Enhanced)' icon on your desktop." -ForegroundColor Yellow
Write-Host "New features include:" -ForegroundColor Cyan
Write-Host "- 3-column layout with Order Entry panel" -ForegroundColor White
Write-Host "- Real-time price updates every 2 seconds" -ForegroundColor White
Write-Host "- BUY/SELL order functionality" -ForegroundColor White
Write-Host "- Enhanced account summary" -ForegroundColor White 