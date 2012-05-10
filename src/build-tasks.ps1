Properties {
    $ThisDir = Split-Path $psake.build_script_file
    $MSBuildConfiguration="Release"
    $NuGetConsole="$ThisDir\..\packages\NuGet.CommandLine.1.7.0\tools\NuGet.exe"
    $XunitConsole35="$ThisDir\..\packages\xunit.runners.1.9.0.1566\tools\xunit.console.exe"
    $XunitConsole40="$ThisDir\..\packages\xunit.runners.1.9.0.1566\tools\xunit.console.clr4.exe"
}

Task Default -depends Package

Task Package -depends Test {
    if (Test-Path "$ThisDir\bin")
    {
        rd "$ThisDir\bin" -Force -Recurse
    }
    
    mkdir "$ThisDir\bin\lib\net35"
    mkdir "$ThisDir\bin\lib\net40"
    
    copy "$ThisDir\Xbehave.Net35\bin\$MSBuildConfiguration\Xbehave.dll" "$ThisDir\bin\lib\net35"
    copy "$ThisDir\Xbehave.Net40\bin\$MSBuildConfiguration\Xbehave.dll" "$ThisDir\bin\lib\net40"
    
    if (Test-Path "$ThisDir\Xbehave.Net35\bin\$MSBuildConfiguration\Xbehave.xml")
    {
        copy "$ThisDir\Xbehave.Net35\bin\$MSBuildConfiguration\Xbehave.xml" "$ThisDir\bin\lib\net35"
    }
    
    if (Test-Path "$ThisDir\Xbehave.Net40\bin\$MSBuildConfiguration\Xbehave.xml")
    {    
        copy "$ThisDir\Xbehave.Net40\bin\$MSBuildConfiguration\Xbehave.xml" "$ThisDir\bin\lib\net40"
    }
    
    Exec { .$NuGetConsole pack "Xbehave.nuspec" -BasePath "$ThisDir\bin" -OutputDirectory "$ThisDir\bin" }
}

Task Test -depends Build {
    Exec { .$XunitConsole35 "$ThisDir\test\Xbehave.Test.Net35\bin\Debug\Xbehave.Test.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Net35\bin\Debug\XBehave.Test.Results.xml" /html "$ThisDir\test\Xbehave.Test.Net35\bin\Debug\XBehave.Test.Results.html" }
    Exec { .$XunitConsole40 "$ThisDir\test\Xbehave.Test.Net40\bin\Debug\Xbehave.Test.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Net40\bin\Debug\XBehave.Test.Results.xml" /html "$ThisDir\test\Xbehave.Test.Net40\bin\Debug\XBehave.Test.Results.html" }
}

Task Build -depends Clean {
    Exec { msbuild "$ThisDir\Xbehave.sln" /t:Build /p:Configuration=$MSBuildConfiguration }
}

Task Clean -depends ResolveVariables {
    Exec { msbuild "$ThisDir\Xbehave.sln" /t:Clean /p:Configuration=$MSBuildConfiguration }
}

Task ResolveVariables {
    Write-Host "ThisDir = $ThisDir"
    Write-Host "MSBuildConfiguration = $MSBuildConfiguration"
    Write-Host "NuGetConsole = $NuGetConsole"
    Write-Host "XunitConsole35 = $XunitConsole35"
    Write-Host "XunitConsole40 = $XunitConsole40"
}