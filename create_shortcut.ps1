# Create Desktop Shortcut for IBKR Trading Desktop v2.0
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$env:USERPROFILE\Desktop\IBKR Trading Desktop v2.0.lnk")
$Shortcut.TargetPath = "C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe"
$Shortcut.WorkingDirectory = "C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\"
$Shortcut.Description = "IBKR Trading Desktop v2.0 - Advanced Trading Application with Dynamic Risk Management"
$Shortcut.IconLocation = "C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe,0"
$Shortcut.Save()

Write-Host "Desktop shortcut created successfully!" -ForegroundColor Green
Write-Host "Shortcut location: $env:USERPROFILE\Desktop\IBKR Trading Desktop v2.0.lnk" -ForegroundColor Yellow
Write-Host "Application path: C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe" -ForegroundColor Cyan 