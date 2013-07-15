$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Import-Module $scriptPath\bin\Debug\Win8Toolkit.dll
$app = Get-WindowsStoreApps | select -last 1
$app.Launch()
$app.SwitchTo()

