$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Import-Module $scriptPath\bin\Debug\Win8Toolkit.dll
$app = Get-AppsFromAppxPackage | select -first 1
$app.SwitchTo()

