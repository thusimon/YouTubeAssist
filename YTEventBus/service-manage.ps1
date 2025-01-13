# Require PowserShell 7.4 - check with $PSVersionTable
# Run as administrator

# publish, can do it in Visual Studio -> Project right click
# dotnet publish -c Release -r win-x64 --self-contained true

param (
  [string]$Action,  # Action to perform: create, remove, start, stop, recreate, restart
  [string]$ServiceName = "YTEventBusService",  # Default service name
  [string]$BinaryPath = "D:\Github\YouTubeAssist\YTEventBus\bin\Debug\net8.0\YTEventBus.exe"  # Default path to service executable
)

# Function to create the service
function CreateService {
  if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) {
    Write-Host "Service '$ServiceName' already exists."
  } else {
    New-Service -Name $ServiceName `
      -BinaryPathName $BinaryPath `
      -DisplayName "YouTube Event Bus Service" `
      -Description "Service manages YouTube events and actions and communicate with other apps" `
      -StartupType Automatic
    Write-Host "Service '$ServiceName' created successfully."
  }
}

# Function to delete the service
function DeleteService {
    # Check if the service exists
    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($service) {
        # Stop the service if it is running
        if ($service.Status -eq "Running") {
            Stop-Service -Name $ServiceName -Force
            Write-Host "Service '$ServiceName' stopped successfully."
        }

        # Remove the service
        Remove-Service -Name $ServiceName | Out-Null
        Write-Host "Service '$ServiceName' removed successfully."

        # Verify removal
        if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) {
            Write-Host "Service '$ServiceName' was not fully removed. Please check manually."
        }
    } else {
        Write-Host "Service '$ServiceName' does not exist."
    }
}

# Function to recreate the service
function RecreateService {
  # Remove the service if it exists
  DeleteService

  # Add a delay to ensure the service is fully removed
  Write-Host "Sleep 4s to wait fully removed"
  Start-Sleep -Seconds 4

  # Recreate the service
  CreateService
}

# Function to start the service
function LaunchService {
  if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) {
    Start-Service -Name $ServiceName
    Write-Host "Service '$ServiceName' started successfully."
  } else {
    Write-Host "Service '$ServiceName' does not exist."
  }
}

# Function to stop the service
function EndService {
  if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) {
    Stop-Service -Name $ServiceName
    Write-Host "Service '$ServiceName' stopped successfully."
  } else {
    Write-Host "Service '$ServiceName' does not exist."
  }
}

# Function to restart service
function RestartService {
  # Stop the service if it exists
  EndService

  # Add a delay to ensure the service is fully stopped
  Write-Host "Sleep 4s to wait fully stopped"
  Start-Sleep -Seconds 4

  # Recreate the service
  LaunchService
}

# Action handling
switch ($Action) {
  "create" {
    CreateService
  }
  "remove" {
    DeleteService
  }
  "recreate" {
    RecreateService
  }
  "start" {
    LaunchService
  }
  "stop" {
    EndService
  }
  "restart" {
    RestartService
  }
  default {
    Write-Host "Invalid action. Please use 'create', 'remove', 'recreate', 'start', 'stop' or 'restart'"
  }
}
