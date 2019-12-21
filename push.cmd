@echo Off

if [%MYGET_ADAMRALPH_CI_API_KEY%] == [] (
  echo "%~nx0: MYGET_ADAMRALPH_CI_API_KEY is empty or not set. Skipped pushing package(s)."
) else (
  forfiles /m *.nupkg /s /c "cmd /c echo %~nx0: Pushing @path && dotnet nuget push @path --source https://www.myget.org/F/adamralph-ci/api/v2/package --api-key %MYGET_ADAMRALPH_CI_API_KEY% }}"
)
