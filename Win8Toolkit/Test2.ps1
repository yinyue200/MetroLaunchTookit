param(
[string]$Name=$(throw "Parameter missing: -name Name") ,
[string]$Path=$(throw "Parameter missing: -path Path")
)
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Import-Module $scriptPath\bin\Debug\Win8Toolkit.dll
# (Get-Module -ListAvailable $scriptPath\bin\Debug\Win8Toolkit.dll).ExportedCmdlets

$app = Get-AppxPackage | Where { $_.Name -match $Name } | Get-WindowsStoreApps | select -last 1
$app.LaunchForFile($Path)
