# Run as administrator

# publish, can do it in Visual Studio -> Project right click
# dotnet publish -c Release -r win-x64 --self-contained true

# Register
New-Service -Name "YTEventBusService" `
  -BinaryPathName "D:\Github\YouTubeAssist\YTEventBus\bin\Debug\net8.0\YTEventBus.exe" `
  -DisplayName "YouTube Event Bus Service" `
  -StartupType Automatic

# Remove
#Remove-Service -Name "YTEventBusService"

# Start
Start-Service -Name "YTEventBusService"

# Stop
#Stop-Service -Name "YTEventBusService"
