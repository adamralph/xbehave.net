#load "packages/simple-targets-csharp.1.1.0/simple-targets-csharp.csx"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

// version
var versionSuffix = Environment.GetEnvironmentVariable("VERSION_SUFFIX") ?? "";
var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "000000";
var buildNumberSuffix = versionSuffix == "" ? "" : "-build" + buildNumber;
var version = File.ReadAllText("src/CommonAssemblyInfo.cs")
    .Split(new[] { "AssemblyInformationalVersion(\"" }, 2, StringSplitOptions.RemoveEmptyEntries)[1]
    .Split('\"').First() + versionSuffix + buildNumberSuffix;

// locations
var solution = "./Xbehave.sln";
var logs = "./artifacts/logs";
var msBuild = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}/MSBuild/14.0/Bin/msbuild.exe";
var nuspecs = new[] { "./src/Xbehave.Core.nuspec", "./src/Xbehave.nuspec", };
var output = "./artifacts/output";
var nuget = "./.nuget/NuGet.exe";
var acceptanceTests = Path.GetFullPath("./tests/Xbehave.Test.Acceptance.Net45/bin/Release/Xbehave.Test.Acceptance.Net45.dll");
var xunit = "./packages/xunit.runner.console.2.1.0/tools/xunit.console.exe";

// targets
var targets = new Dictionary<string, Target>();

targets.Add("default", new Target { DependOn = new[] { "pack", "accept" } });

targets.Add("logs", new Target { Outputs = new[] { logs }, Do = () => Directory.CreateDirectory(logs), });

targets.Add(
    "build",
    new Target
    {
        Inputs = new[] { logs },
        Do = () => Cmd(
            msBuild,
            $"{solution} /p:Configuration=Release /nologo /m /v:m /nr:false " +
                $"/fl /flp:LogFile={logs}/msbuild.log;Verbosity=Detailed;PerformanceSummary"),
    });

targets.Add("output", new Target { Outputs = new[] { output }, Do = () => Directory.CreateDirectory(output), });

targets.Add(
    "pack",
    new Target
    {
        DependOn = new[] { "build" },
        Inputs = new[] { output },
        Do = () =>
        {
            foreach (var nuspec in nuspecs)
            {
                var originalNuspec = $"{nuspec}.original";
                File.Move(nuspec, originalNuspec);
                var originalContent = File.ReadAllText(originalNuspec);
                var content = originalContent.Replace("[0.0.0]", $"[{version}]");
                File.WriteAllText(nuspec, content);
                try
                {
                    Cmd(nuget, $"pack {nuspec} -Version {version} -OutputDirectory {output} -NoPackageAnalysis");
                }
                finally
                {
                    File.Delete(nuspec);
                    File.Move(originalNuspec, nuspec);
                }
            }
        },
    });

targets.Add(
    "accept",
    new Target
    {
        DependOn = new[] { "build" },
        Do = () => Cmd(
            xunit, $"{acceptanceTests} -html {acceptanceTests}.TestResults.html -xml {acceptanceTests}.TestResults.xml"),
    });

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
