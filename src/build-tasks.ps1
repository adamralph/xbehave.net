Properties {
    $MSBuildConfiguration="Release"
    $XunitConsole="..\packages\xunit.runners.1.9.0.1566\tools\xunit.console.clr4.exe"
    
    Write-Host "MSBuildConfiguration = $MSBuildConfiguration"
    Write-Host "XunitConsole = $XunitConsole"
}

Task Default -depends Test

Task Test -depends Build {
    Exec { .$XunitConsole "test\Xbehave.Test\bin\Debug\Xbehave.Test.dll" /noshadow /nunit "test\Xbehave.Test\bin\Debug\XBehave.Test.Results.Xml" }
}

Task Build -depends Clean {
    Exec { msbuild "Xbehave.sln" /t:Build /p:Configuration=$MSBuildConfiguration }
}

Task Clean {
    Exec { msbuild "Xbehave.sln" /t:Clean /p:Configuration=$MSBuildConfiguration }
}