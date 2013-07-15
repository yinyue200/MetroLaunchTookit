$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Import-Module $scriptPath\bin\Debug\Win8Toolkit.dll
$app = Get-WindowsStoreApps | select -last 1
if ($app.ExecutionState -ne "Terminated") {
  $app.Terminate()
}
$app.Launch()
Get-AppOrStartVisibility
sleep 2
Get-AppOrStartVisibility
sleep 1
$app.SwitchTo()
Get-AppOrStartVisibility

