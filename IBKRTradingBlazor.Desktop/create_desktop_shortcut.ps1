# Create Desktop Shortcut for IBKR Trading Desktop
Write-Host "Creating desktop shortcut..." -ForegroundColor Green

# Get the current directory
$CurrentDir = Get-Location
$ExePath = Join-Path $CurrentDir "bin\Debug\net9.0-windows10.0.19041.0\win10-x64\IBKRTradingBlazor.Desktop.exe"

# Check if executable exists
if (Test-Path $ExePath) {
    Write-Host "Executable found at: $ExePath" -ForegroundColor Green
    
    # Create shortcut on desktop
    $DesktopPath = [Environment]::GetFolderPath("Desktop")
    $ShortcutPath = Join-Path $DesktopPath "IBKR Trading Desktop.lnk"
    
    $WshShell = New-Object -ComObject WScript.Shell
    $Shortcut = $WshShell.CreateShortcut($ShortcutPath)
    $Shortcut.TargetPath = $ExePath
    $Shortcut.WorkingDirectory = Split-Path $ExePath
    $Shortcut.Description = "IBKR Trading Desktop Application"
    $Shortcut.Save()
    
    Write-Host "Desktop shortcut created successfully at: $ShortcutPath" -ForegroundColor Green
    Write-Host "You can now double-click the 'IBKR Trading Desktop' icon on your desktop to start the application." -ForegroundColor Yellow
} else {
    Write-Host "Error: Executable not found at $ExePath" -ForegroundColor Red
    Write-Host "Please build the application first using: dotnet build --framework net9.0-windows10.0.19041.0 --configuration Debug" -ForegroundColor Yellow
} 