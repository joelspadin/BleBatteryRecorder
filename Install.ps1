#Requires -RunAsAdministrator

param(
    [switch]$Debug
)

# Remove existing package
& "$PSScriptRoot/Uninstall.ps1"

# Build package
$configuration = if ($Debug) { 'Debug' } else { 'Release' }
dotnet publish -c $configuration

$csprojPath = "$PSScriptRoot/BleBatteryRecorder.csproj"

$framework = Select-Xml -LiteralPath "$csprojPath" -XPath '//TargetFramework'
| Select-Object -ExpandProperty 'Node'
| Select-Object -ExpandProperty '#text'

$appmanifestTemplate = "$PSScriptRoot/appxmanifest.xml"
$appmanifestPath = "$PSScriptRoot/bin/appxmanifest.xml"
$binaryPath = "$configuration/$framework/win-x64/publish/BleBatteryRecorder.exe"

(Get-Content "$appmanifestTemplate") -replace 'BleBatteryRecorder.exe', "$binaryPath"
| Out-File -LiteralPath "$appmanifestPath"

# Install new package
Add-AppxPackage -Path "$appmanifestPath" -Register
