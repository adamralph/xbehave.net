Properties {
    $ThisDir = Split-Path $psake.build_script_file
    $MSBuildConfiguration="Release"
    $NuGetConsole="$ThisDir\..\packages\NuGet.CommandLine.2.1.2\tools\NuGet.exe"
    $XunitConsole35="$ThisDir\..\packages\xunit.runners.1.9.1\tools\xunit.console.exe"
    $XunitConsole40="$ThisDir\..\packages\xunit.runners.1.9.1\tools\xunit.console.clr4.exe"
    $XunitConsole45="$ThisDir\..\packages\xunit.runners.2.0.0-alpha-build1611\tools\xunit.console.exe"
}

Task Default -depends Package

Task Package -depends Test {
    if (Test-Path "$ThisDir\bin")
    {
        rd "$ThisDir\bin" -Force -Recurse
    }
    
    mkdir "$ThisDir\bin\lib\net35"
    copy "$ThisDir\Xbehave.Net35\bin\$MSBuildConfiguration\Xbehave.dll" "$ThisDir\bin\lib\net35"
    if (Test-Path "$ThisDir\Xbehave.Net35\bin\$MSBuildConfiguration\Xbehave.xml")
    {
        copy "$ThisDir\Xbehave.Net35\bin\$MSBuildConfiguration\Xbehave.xml" "$ThisDir\bin\lib\net35"
    }
  
    mkdir "$ThisDir\bin\lib\net40"
    copy "$ThisDir\Xbehave.Net40\bin\$MSBuildConfiguration\Xbehave.dll" "$ThisDir\bin\lib\net40"
    if (Test-Path "$ThisDir\Xbehave.Net40\bin\$MSBuildConfiguration\Xbehave.xml")
    {    
        copy "$ThisDir\Xbehave.Net40\bin\$MSBuildConfiguration\Xbehave.xml" "$ThisDir\bin\lib\net40"
    }
    
    mkdir "$ThisDir\bin\lib\net45"
    copy "$ThisDir\Xbehave.Net45\bin\$MSBuildConfiguration\Xbehave.dll" "$ThisDir\bin\lib\net45"
    if (Test-Path "$ThisDir\Xbehave.Net45\bin\$MSBuildConfiguration\Xbehave.xml")
    {    
        copy "$ThisDir\Xbehave.Net45\bin\$MSBuildConfiguration\Xbehave.xml" "$ThisDir\bin\lib\net45"
    }

    Exec { .$NuGetConsole pack "Xbehave.nuspec" -BasePath "$ThisDir\bin" -OutputDirectory "$ThisDir\bin" }
}

Task Test -depends Build {
    Exec { .$XunitConsole35 "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net35\bin\Debug\Xbehave.Sdk.Test.Unit.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net35\bin\Debug\Xbehave.Sdk.Test.Unit.Results.xml" /html "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net35\bin\Debug\Xbehave.Sdk.Test.Unit.Results.html" }
    Exec { .$XunitConsole40 "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net40\bin\Debug\Xbehave.Sdk.Test.Unit.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net40\bin\Debug\Xbehave.Sdk.Test.Unit.Results.xml" /html "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net40\bin\Debug\Xbehave.Sdk.Test.Unit.Results.html" }
    Exec { .$XunitConsole45 "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net45\bin\Debug\Xbehave.Sdk.Test.Unit.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net45\bin\Debug\Xbehave.Sdk.Test.Unit.Results.xml" /html "$ThisDir\test\Xbehave.Sdk.Test.Unit.Net45\bin\Debug\Xbehave.Sdk.Test.Unit.Results.html" }
    
    Exec { .$XunitConsole35 "$ThisDir\test\Xbehave.Test.Unit.Net35\bin\Debug\Xbehave.Test.Unit.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Unit.Net35\bin\Debug\Xbehave.Test.Unit.Results.xml" /html "$ThisDir\test\Xbehave.Test.Unit.Net35\bin\Debug\Xbehave.Test.Unit.Results.html" }
    Exec { .$XunitConsole40 "$ThisDir\test\Xbehave.Test.Unit.Net40\bin\Debug\Xbehave.Test.Unit.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Unit.Net40\bin\Debug\Xbehave.Test.Unit.Results.xml" /html "$ThisDir\test\Xbehave.Test.Unit.Net40\bin\Debug\Xbehave.Test.Unit.Results.html" }
    Exec { .$XunitConsole45 "$ThisDir\test\Xbehave.Test.Unit.Net45\bin\Debug\Xbehave.Test.Unit.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Unit.Net45\bin\Debug\Xbehave.Test.Unit.Results.xml" /html "$ThisDir\test\Xbehave.Test.Unit.Net45\bin\Debug\Xbehave.Test.Unit.Results.html" }
    
    Exec { .$XunitConsole35 "$ThisDir\test\Xbehave.Test.Acceptance.Net35\bin\Debug\Xbehave.Test.Acceptance.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Acceptance.Net35\bin\Debug\Xbehave.Test.Acceptance.Results.xml" /html "$ThisDir\test\Xbehave.Test.Acceptance.Net35\bin\Debug\Xbehave.Test.Acceptance.Results.html" }
    Exec { .$XunitConsole40 "$ThisDir\test\Xbehave.Test.Acceptance.Net40\bin\Debug\Xbehave.Test.Acceptance.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Acceptance.Net40\bin\Debug\Xbehave.Test.Acceptance.Results.xml" /html "$ThisDir\test\Xbehave.Test.Acceptance.Net40\bin\Debug\Xbehave.Test.Acceptance.Results.html" }
    Exec { .$XunitConsole45 "$ThisDir\test\Xbehave.Test.Acceptance.Net45\bin\Debug\Xbehave.Test.Acceptance.dll" /noshadow /nunit "$ThisDir\test\Xbehave.Test.Acceptance.Net45\bin\Debug\Xbehave.Test.Acceptance.Results.xml" /html "$ThisDir\test\Xbehave.Test.Acceptance.Net45\bin\Debug\Xbehave.Test.Acceptance.Results.html" }
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
    Write-Host "XunitConsole45 = $XunitConsole45"
}