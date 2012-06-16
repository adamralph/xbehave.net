$ThisDir = Split-Path ((Get-Variable MyInvocation -Scope 0).Value).MyCommand.Path
Import-Module $ThisDir\..\packages\psake.4.2.0.1\tools\psake.psm1 -force
Invoke-psake $ThisDir\build-tasks.ps1 -properties  @{"MSBuildConfiguration"="Release"}
Remove-Module psake