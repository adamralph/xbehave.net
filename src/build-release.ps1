$ThisDir = Split-Path ((Get-Variable MyInvocation -Scope 0).Value).MyCommand.Path
Import-Module $ThisDir\..\packages\psake.4.1.0\tools\psake.psm1 -force
Invoke-psake $ThisDir\build-tasks.ps1 -properties  @{"MSBuildConfiguration"="Release"}
Remove-Module psake