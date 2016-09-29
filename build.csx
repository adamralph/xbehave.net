#load "packages/simple-targets-csx.5.0.0/simple-targets.csx"

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static SimpleTargets;

var targets = new TargetDictionary();

targets.Add("default", DependsOn("pack", "test"));

targets.Add("restore", () => Cmd("dotnet", "restore"));

targets.Add("build", () => Cmd("dotnet", $"build ./**/project.json -c Release"));

targets.Add(
    "pack",
    DependsOn("build"),
    () =>
    {
        var versionSuffix = Environment.GetEnvironmentVariable("VERSION_SUFFIX") ?? "-adhoc";
        var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "000000";
        var buildNumberSuffix = versionSuffix == "" ? "" : "-build" + buildNumber;
        var version = File.ReadAllText("src/CommonAssemblyInfo.cs")
            .Split(new[] { "AssemblyInformationalVersion(\"" }, 2, StringSplitOptions.RemoveEmptyEntries)[1]
            .Split('\"').First() + versionSuffix + buildNumberSuffix;

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
                Cmd("./.nuget/NuGet.exe", $"pack {nuspec} -Version {version} -OutputDirectory ./artifacts -NoPackageAnalysis");
            }
            finally
            {
                File.Delete(nuspec);
                File.Move(originalNuspec, nuspec);
            }
        }
    });

targets.Add("test", () => Cmd("dotnet", $"test ./tests/Xbehave.Test -c Release"));

Run(Args, targets);

// helper
public static void Cmd(string fileName, string args)
{
    using (var process = new Process())
    {
        process.StartInfo = new ProcessStartInfo { FileName = $"\"{fileName}\"", Arguments = args, UseShellExecute = false, };
        Console.WriteLine($"Running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'...");
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"The command exited with code {process.ExitCode}.");
        }
    }
}
