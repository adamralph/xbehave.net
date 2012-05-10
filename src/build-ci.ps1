$ThisDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
Import-Module $ThisDir\..\packages\psake.4.1.0\tools\psake.psm1 -force
Invoke-psake $ThisDir\build-tasks.ps1 -properties  @{"MSBuildConfiguration"="CI"}
Remove-Module psake