#Requires -RunAsAdministrator

# Stop the service if it's already running
$service = Get-Service 'BLE Battery Recorder' -ErrorAction SilentlyContinue

if ($service -and ($service.Status -eq 'Running')) {
    $service.Stop()
}

Get-AppxPackage -Name '*JoelSpadin.BleBatteryRecorder*' | ForEach-Object {
    Remove-AppxPackage $_.PackageFullName
}
