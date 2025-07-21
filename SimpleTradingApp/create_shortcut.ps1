$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$env:USERPROFILE\Desktop\IBKR Trading App.lnk")
$Shortcut.TargetPath = "C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\SimpleTradingApp.exe"
$Shortcut.WorkingDirectory = "C:\IBKRTradingBlazor\SimpleTradingApp\bin\Release\net9.0-windows\win-x64\publish\"
$Shortcut.Description = "IBKR Trading Desktop Application"
$Shortcut.Save()

Write-Host "Desktop shortcut created successfully!" -ForegroundColor Green
Write-Host "You can now double-click the 'IBKR Trading App' icon on your desktop to start the application." -ForegroundColor Yellow 