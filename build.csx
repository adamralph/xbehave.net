#load "packages/simple-targets-csx.5.3.0/simple-targets.csx"

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static SimpleTargets;

var targets = new TargetDictionary();

targets.Add("default", DependsOn("pack", "test"));

targets.Add("build", () => Cmd("dotnet", $"build -c Release"));

targets.Add(
    "pack",
    DependsOn("build"),
    () =>
    {
        var versionSuffix = Environment.GetEnvironmentVariable("VERSION_SUFFIX") ?? "-adhoc";
        var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "000000";
        var buildNumberSuffix = versionSuffix == "" ? "" : "-build" + buildNumber;
        var version = File.ReadAllText("src/Directory.Build.props")
                .Split(new[] { "<Version>" }, 2, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(new[] { "</Version>" }, StringSplitOptions.RemoveEmptyEntries).First()
            + versionSuffix + buildNumberSuffix;

        Directory.CreateDirectory("./artifacts");

        foreach (var nuspec in Directory.EnumerateFiles("./src", "*.nuspec"))
        {
            var originalNuspec = $"{nuspec}.original";
            File.Move(nuspec, originalNuspec);
            var originalContent = File.ReadAllText(originalNuspec);
            var content = originalContent.Replace("[99.99.99-dev]", $"[{version}]");
            File.WriteAllText(nuspec, content);
            try
            {
                Cmd("./.nuget/v4.3.0/NuGet.exe", $"pack {nuspec} -Version {version} -OutputDirectory ./artifacts -NoPackageAnalysis");
            }
            finally
            {
                File.Delete(nuspec);
                File.Move(originalNuspec, nuspec);
            }
        }
    });

targets.Add(
    "test-core",
    DependsOn("build"),
    () => Cmd("dotnet", $"xunit -configuration Release -nobuild -framework netcoreapp1.1", "./tests/Xbehave.Test"));

targets.Add(
    "test-net",
    DependsOn("build"),
    () => Cmd("dotnet", $"xunit -configuration Release -nobuild -framework net452", "./tests/Xbehave.Test"));

targets.Add("test", DependsOn("test-core", "test-net"));

Run(Args, targets);

// helper
public static void Cmd(string fileName, string args) => Cmd(fileName, args, "");

public static void Cmd(string fileName, string args, string workingDirectory)
{
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo
        {
            FileName = $"\"{fileName}\"",
            Arguments = args,
            UseShellExecute = false,
            WorkingDirectory = workingDirectory,
        };

        Console.WriteLine($"Running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'...");
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"The command exited with code {process.ExitCode}.");
        }
    }
}
