using System;
using System.Threading.Tasks;
using SimpleExec;
using static Bullseye.Targets;
using static SimpleExec.Command;

internal class Program
{
    public static Task Main(string[] args)
    {
        Target("default", DependsOn("pack", "test"));

        Target("build", () => RunAsync("dotnet", $"build --configuration Release"));

        Target(
            "pack",
            DependsOn("build"),
            ForEach("Xbehave.Core.nuspec", "Xbehave.nuspec"),
            async nuspec =>
            {
                Environment.SetEnvironmentVariable("NUSPEC_FILE", nuspec, EnvironmentVariableTarget.Process);
                await RunAsync("dotnet", $"pack src/Xbehave.Core --configuration Release --no-build");
            });

        Target(
            "test-core",
            DependsOn("build"),
            () => RunAsync("dotnet", $"test ./tests/Xbehave.Test/Xbehave.Test.csproj --configuration Release --no-build --framework netcoreapp2.1"));

        Target(
            "test-net",
            DependsOn("build"),
            () => RunAsync("dotnet", $"test ./tests/Xbehave.Test/Xbehave.Test.csproj --configuration Release --no-build --framework net472"));

        Target("test", DependsOn("test-core", "test-net"));

        return RunTargetsAndExitAsync(args, ex => ex is NonZeroExitCodeException);
    }
}
