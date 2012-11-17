$ThisDir = Split-Path ((Get-Variable MyInvocation -Scope 0).Value).MyCommand.Path
Import-Module $ThisDir\..\packages\psake.4.2.0.1\tools\psake.psm1 -force
Invoke-psake $ThisDir\BuildTasks.ps1 -properties  @{"MSBuildConfiguration"="Debug"}
$build_success = $psake.build_success
Remove-Module psake
if ($build_success -eq $false) { exit 1 } else { exit 0 }