@echo Off

for /r %%package in ("*.nupkg") do echo %~nx0: Pushing %%package... && dotnet nuget push %%package --source https://www.myget.org/F/adamralph-ci/api/v2/package --api-key %1
