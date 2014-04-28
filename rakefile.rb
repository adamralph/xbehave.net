require 'albacore'
require 'fileutils'

version = IO.read("src/VersionInfo.1.cs").split(/AssemblyInformationalVersion\("/, 2)[1].split(/"/).first
xunit_command = "src/packages/xunit.runners.2.0.0-beta-build2616/tools/xunit.console.exe"
nuget_command = "src/packages/NuGet.CommandLine.2.8.1/tools/NuGet.exe"
solution = "src/XBehave.sln"
output = "bin"

specs = [
  "src/test/Xbehave.Sdk.Specifications.Net35/bin/Release/Xbehave.Sdk.Specifications.Net35.dll",
  "src/test/Xbehave.Specifications.Net35/bin/Release/Xbehave.Specifications.Net35.dll",
  "src/test/Xbehave.Sdk.Specifications.Net40/bin/Release/Xbehave.Sdk.Specifications.Net40.dll",
  "src/test/Xbehave.Specifications.Net40/bin/Release/Xbehave.Specifications.Net40.dll",
]

features = [
  "src/test/Xbehave.Features.Net35/bin/Release/Xbehave.Features.Net35.dll",
  "src/test/Xbehave.Features.Net40/bin/Release/Xbehave.Features.Net40.dll",
  "src/test/Xbehave.Features.Net45/bin/Release/Xbehave.Features.Net45.dll",
  "src/test/Xbehave.Features.2.Net45/bin/Release/Xbehave.Features.2.Net45.dll",
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
  msb.properties = { :configuration => :Release }
  msb.targets = [:Clean]
  msb.solution = solution
end

desc "Build solution"
msbuild :build => [:clean, :restore] do |msb|
  msb.properties = { :configuration => :Release }
  msb.targets = [:Build]
  msb.solution = solution
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
