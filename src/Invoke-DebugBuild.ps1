$ThisDir = Split-Path ((Get-Variable MyInvocation -Scope 0).Value).MyCommand.Path
Import-Module $ThisDir\..\packages\psake.4.2.0.1\tools\psake.psm1 -force
Invoke-psake $ThisDir\BuildTasks.ps1 -properties  @{"MSBuildConfiguration"="Debug"}
Remove-Module psake