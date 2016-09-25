using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        Do = () => Cmd(msBuild, $"{solution} /p:Configuration=Release /nologo /m /v:m /nr:false /fl /flp:LogFile={logs}/msbuild.log;Verbosity=Detailed;PerformanceSummary"),
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

// target running boiler plate
Run(Args, targets);

public class Target
{
    public string[] DependOn { get; set; }

    public string[] Inputs { get; set; }

    public string[] Outputs { get; set; }

    public Action Do { get; set; }
}

public static void Run(IList<string> args, IDictionary<string, Target> targets)
{
    var argsOptions = args.Where(arg => arg.StartsWith("-", StringComparison.Ordinal)).ToList();
    var argsTargets = args.Except(argsOptions).ToList();

    foreach (var option in argsOptions)
    {
        switch (option)
        {
            case "-H":
            case "-h":
            case "-?":
                Console.WriteLine("Usage: <script-runner> build.csx [<options>] [<targets>]");
                Console.WriteLine();
                Console.WriteLine("script-runner: A C# script runner. E.g. csi.exe.");
                Console.WriteLine();
                Console.WriteLine("options:");
                Console.WriteLine(" -T      Display the targets, then exit");
                Console.WriteLine();
                Console.WriteLine("targets: A list of targets to run. If not specified, 'default' target will be run.");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                Console.WriteLine("  csi.exe build.csx");
                Console.WriteLine("  csi.exe build.csx -T");
                Console.WriteLine("  csi.exe build.csx test package");
                return;
            case "-T":
                foreach (var target in targets)
                {
                    Console.WriteLine(target.Key);
                }

                return;
            default:
                Console.WriteLine($"Unknown option '{option}'.");
                return;
        }
    }

    var targetNames = argsTargets.Any() ? argsTargets : new List<string> { "default" };
    var targetsRan = new HashSet<string>();

    targetNames.ForEach(name => RunTarget(name, targets, targetsRan));

    Console.WriteLine($"Target(s) {string.Join(", ", targetNames.Select(name => $"'{name}'"))} succeeded.");
}

public static void RunTarget(string name, IDictionary<string, Target> targets, ISet<string> targetsRan)
{
    Target target;
    if (!targets.TryGetValue(name, out target))
    {
        throw new InvalidOperationException($"Target '{name}' not found.");
    }

    targetsRan.Add(name);

    var outputs = target.Outputs ?? Enumerable.Empty<string>();
    if (outputs.Any() && !outputs.Any(output => !File.Exists(output)))
    {
        Console.WriteLine($"Skipping target '{name}' since all outputs are present.");
        return;
    }

    var inputs = target.Inputs ?? Enumerable.Empty<string>();
    var dependencies = (target.DependOn ?? Enumerable.Empty<string>()).Concat(
            targets.Where(t => (t.Value.Outputs ?? Enumerable.Empty<string>()).Intersect(inputs).Any()).Select(t => t.Key))
        .Except(targetsRan)
        .ToList();

    if (dependencies.Any())
    {
        Console.WriteLine($"Running dependencies for target '{name}'...");
        foreach (var dependency in dependencies)
        {
            RunTarget(dependency, targets, targetsRan);
        }
    }

    if (target.Do != null)
    {
        Console.WriteLine($"Running target '{name}'...");
        target.Do.Invoke();
    }
}

// target writing boiler plate
public static void Cmd(string fileName, string args)
{
    var info = new ProcessStartInfo
    {
        FileName = "\"" + fileName + "\"",
        Arguments = args,
        UseShellExecute = false,
    };

    Console.WriteLine($"Running '{info.FileName} {info.Arguments}'...");

    using (var process = new Process())
    {
        process.StartInfo = info;
        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            var message = string.Format(
                CultureInfo.InvariantCulture,
                "The command exited with code {0}.",
                process.ExitCode.ToString(CultureInfo.InvariantCulture));

            throw new InvalidOperationException(message);
        }
    }
}
