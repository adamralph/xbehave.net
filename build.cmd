:: the windows shell, so amazing

:: options
@echo Off
cd %~dp0
setlocal

:: determine nuget cache dir
set NUGET_CACHE_DIR=%LocalAppData%\.nuget\v3.5.0

:: download nuget to cache dir
set NUGET_URL=https://dist.nuget.org/win-x86-commandline/v3.5.0/NuGet.exe
if not exist %NUGET_CACHE_DIR%\NuGet.exe (
  if not exist %NUGET_CACHE_DIR% md %NUGET_CACHE_DIR%
  echo Downloading '%NUGET_URL%'' to '%NUGET_CACHE_DIR%\NuGet.exe'...
  @powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest '%NUGET_URL%' -OutFile '%NUGET_CACHE_DIR%\NuGet.exe'"
)

:: copy nuget locally
if not exist .nuget\NuGet.exe (
  if not exist .nuget md .nuget
  copy %NUGET_CACHE_DIR%\NuGet.exe .nuget\NuGet.exe > nul
)

:: restore packages for build script
.nuget\NuGet.exe restore .\packages.config -PackagesDirectory ./packages -Verbosity quiet

:: run build script
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\csi.exe" .\build.csx %*
