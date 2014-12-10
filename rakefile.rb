require 'albacore'
require 'fileutils'

version_suffix1 = ENV["VERSION_SUFFIX1"] || ""
version_suffix2 = ENV["VERSION_SUFFIX2"] || "-pre"
build_number_suffix1 = version_suffix1 == "" ? "" : "-build-" + ENV["BUILD_NUMBER"]
build_number_suffix2 = version_suffix2 == "" ? "" : "-build-" + ENV["BUILD_NUMBER"]

version1 = IO.read("src/VersionInfo.1.cs").split(/AssemblyInformationalVersion\("/, 2)[1].split(/"/).first + version_suffix1 + build_number_suffix1
version2 = IO.read("src/VersionInfo.2.cs").split(/AssemblyInformationalVersion\("/, 2)[1].split(/"/).first + version_suffix2 + build_number_suffix2
xunit_command = "src/packages/xunit.runners.2.0.0-beta5-build2785/tools/xunit.console.exe"
nuget_command = "src/packages/NuGet.CommandLine.2.8.2/tools/NuGet.exe"
solution = "src/XBehave.sln"
output = "artifacts/output"
logs = "artifacts/logs"

component_tests = [
  "src/test/Xbehave.Sdk.Test.Component.Net35/bin/Release/Xbehave.Sdk.Test.Component.Net35.dll",
  "src/test/Xbehave.Test.Component.Net35/bin/Release/Xbehave.Test.Component.Net35.dll",
  "src/test/Xbehave.Sdk.Test.Component.Net40/bin/Release/Xbehave.Sdk.Test.Component.Net40.dll",
  "src/test/Xbehave.Test.Component.Net40/bin/Release/Xbehave.Test.Component.Net40.dll",
]

acceptance_tests = [
  "src/test/Xbehave.Test.Acceptance.Net35/bin/Release/Xbehave.Test.Acceptance.Net35.dll",
  "src/test/Xbehave.Test.Acceptance.Net40/bin/Release/Xbehave.Test.Acceptance.Net40.dll",
  "src/test/Xbehave.Test.Acceptance.Net45/bin/Release/Xbehave.Test.Acceptance.Net45.dll",
  "src/test/Xbehave.2.Test.Acceptance.Net45/bin/Release/Xbehave.Test.Acceptance.Net45.dll",
]

nuspecs = [
  { :file => "src/Xbehave.nuspec", :version => version1 },
  { :file => "src/Xbehave.2.nuspec", :version => version2 }
]

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [:component, :accept, :pack]

desc "Restore NuGet packages"
exec :restore do |cmd|
  cmd.command = nuget_command
  cmd.parameters "restore #{solution}"
end

desc "Clean solution"
msbuild :clean do |msb|
  FileUtils.rmtree output
  FileUtils.mkpath logs
  msb.properties = { :configuration => :Release }
  msb.targets = [:Clean]
  msb.solution = solution
  msb.verbosity = :minimal
  msb.other_switches = {:nologo => true, :fl => true, :flp => "LogFile=#{logs}/clean.log;Verbosity=Detailed;PerformanceSummary", :nr => false}
end

desc "Build solution"
msbuild :build => [:clean, :restore] do |msb|
  FileUtils.mkpath logs
  msb.properties = { :configuration => :Release }
  msb.targets = [:Build]
  msb.solution = solution
  msb.verbosity = :minimal
  msb.other_switches = {:nologo => true, :fl => true, :flp => "LogFile=#{logs}/build.log;Verbosity=Detailed;PerformanceSummary", :nr => false}
end

desc "Run component tests"
task :component => [:build] do
  run_tests component_tests, xunit_command
end

desc "Run acceptance tests"
task :accept => [:build] do
  run_tests acceptance_tests, xunit_command
end

desc "Create the nuget packages"
task :pack => [:build] do
  FileUtils.mkpath output
  nuspecs.each do |nuspec|
    cmd = Exec.new
    cmd.command = nuget_command
    cmd.parameters "pack " + nuspec[:file] + " -Version " + nuspec[:version] + " -OutputDirectory " + output + " -NoPackageAnalysis"
    cmd.execute
  end
end

def run_tests(tests, command)
  tests.each do |test|
    xunit = XUnitTestRunner.new
    xunit.command = command
    xunit.assembly = test
    xunit.options "-html", File.expand_path(test + ".TestResults.html"), "-xml", File.expand_path(test + ".TestResults.xml")
    xunit.execute  
  end
end
