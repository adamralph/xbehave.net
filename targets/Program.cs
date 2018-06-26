// <copyright file="Program.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;

using static Bullseye.Targets;
using static SimpleExec.Command;

internal class Program
{
    public static Task Main(string[] args)
    {
        Add("default", DependsOn("pack", "test"));

        Add("build", () => RunAsync("dotnet", $"build --configuration Release"));

        Add(
            "pack",
            DependsOn("build"),
            async () =>
            {
                foreach (var nuspec in new[] { "Xbehave.Core.nuspec", "Xbehave.nuspec", })
                {
                    Environment.SetEnvironmentVariable("NUSPEC_FILE", nuspec, EnvironmentVariableTarget.Process);
                    await RunAsync("dotnet", $"pack src/Xbehave.Core --configuration Release --no-build");
                }
            });

        Add(
            "test-core",
            DependsOn("build"),
            () => RunAsync("dotnet", $"test ./tests/Xbehave.Test/Xbehave.Test.csproj --configuration Release --no-build --framework netcoreapp2.0"));

        Add(
            "test-net",
            DependsOn("build"),
            () => RunAsync("dotnet", $"test ./tests/Xbehave.Test/Xbehave.Test.csproj --configuration Release --no-build --framework net452"));

        Add("test", DependsOn("test-core", "test-net"));

        return RunAsync(args);
    }
}
