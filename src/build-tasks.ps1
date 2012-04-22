Properties {
    $MSBuildConfiguration="Release"
    Write-Host "MSBuildConfiguration = $MSBuildConfiguration"
}

Task Default -depends Build

Task Build -depends Clean {
    Exec { msbuild "Xbehave.sln" /p:Configuration=$MSBuildConfiguration }
}

Task Clean {
    Exec { msbuild "Xbehave.sln" /t:clean /p:Configuration=$MSBuildConfiguration }
}