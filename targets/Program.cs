using static Bullseye.Targets;
using static SimpleExec.Command;

Target("build", () => RunAsync("dotnet", "build --configuration Release --nologo --verbosity quiet"));

Target(
    "pack",
    DependsOn("build"),
    ForEach("Xbehave.Core.nuspec", "Xbehave.nuspec"),
    async nuspec => await RunAsync(
        "dotnet",
        "pack src/Xbehave.Core --configuration Release --no-build --nologo",
        configureEnvironment: env => env.Add("NUSPEC_FILE", nuspec)));

Target(
    "test",
    DependsOn("build"),
    () => RunAsync("dotnet", "test --configuration Release --no-build --nologo"));

Target("default", DependsOn("pack", "test"));

await RunTargetsAndExitAsync(args, ex => ex is SimpleExec.NonZeroExitCodeException);
