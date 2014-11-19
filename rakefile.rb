require 'albacore'
require 'fileutils'

version = IO.read("src/VersionInfo.1.cs").split(/AssemblyInformationalVersion\("/, 2)[1].split(/"/).first
xunit_command = "src/packages/xunit.runners.2.0.0-beta4-build2738/tools/xunit.console.exe"
nuget_command = "src/packages/NuGet.CommandLine.2.8.2/tools/NuGet.exe"
solution = "src/XBehave.sln"
output = "artifacts/output"
logs = "artifacts/logs"

specs = [
  "src/test/Xbehave.Sdk.Test.Unit.Net35/bin/Release/Xbehave.Sdk.Test.Unit.Net35.dll",
  "src/test/Xbehave.Test.Unit.Net35/bin/Release/Xbehave.Test.Unit.Net35.dll",
  "src/test/Xbehave.Sdk.Test.Unit.Net40/bin/Release/Xbehave.Sdk.Test.Unit.Net40.dll",
  "src/test/Xbehave.Test.Unit.Net40/bin/Release/Xbehave.Test.Unit.Net40.dll",
]

features = [
  "src/test/Xbehave.Test.Acceptance.Net35/bin/Release/Xbehave.Test.Acceptance.Net35.dll",
  "src/test/Xbehave.Test.Acceptance.Net40/bin/Release/Xbehave.Test.Acceptance.Net40.dll",
  "src/test/Xbehave.Test.Acceptance.Net45/bin/Release/Xbehave.Test.Acceptance.Net45.dll",
  "src/test/Xbehave.2.Test.Acceptance.Net45/bin/Release/Xbehave.Test.Acceptance.Net45.dll",
]

nuspec = "src/Xbehave.nuspec"

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [:spec, :feature, :pack]

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

desc "Execute specs"
task :spec => [:build] do
  execute_xunit specs, xunit_command
end

desc "Execute features"
task :feature => [:build] do
  execute_xunit features, xunit_command
end

desc "Create the nuget package"
exec :pack => [:build] do |cmd|
  FileUtils.mkpath output
  cmd.command = nuget_command
  cmd.parameters "pack " + nuspec + " -Version " + version + " -OutputDirectory " + output
end

def execute_xunit(tests, command)
  tests.each do |test|
    xunit = XUnitTestRunner.new
    xunit.command = command
    xunit.assembly = test
    xunit.options "-html", File.expand_path(test + ".TestResults.html"), "-xml", File.expand_path(test + ".TestResults.xml")
    xunit.execute  
  end
end
