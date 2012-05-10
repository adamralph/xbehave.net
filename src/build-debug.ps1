Split-Path -Path $MyInvocation.MyCommand.Definition -Parent | Set-Location
Import-Module ..\packages\psake.4.1.0\tools\psake.psm1 -force
Invoke-psake .\build-tasks.ps1 -properties  @{"MSBuildConfiguration"="Debug"}
Remove-Module psake