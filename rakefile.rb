require 'albacore'
require 'fileutils'

xunit_command_net20 = "packages/xunit.runners.1.9.1/tools/xunit.console.exe"
xunit_command_net40 = "packages/xunit.runners.1.9.1/tools/xunit.console.clr4.exe"
nuget_command = "packages/NuGet.CommandLine.2.2.0/tools/NuGet.exe"
solution = "src/XBehave.sln"
output = "bin"

specs = [
  { :command => xunit_command_net20, :assembly => "src/test/Xbehave.Sdk.Specifications.Net35/bin/Debug/Xbehave.Sdk.Specifications.Net35.dll" },
  { :command => xunit_command_net20, :assembly => "src/test/Xbehave.Specifications.Net35/bin/Debug/Xbehave.Specifications.Net35.dll" },
  { :command => xunit_command_net40, :assembly => "src/test/Xbehave.Sdk.Specifications.Net40/bin/Debug/Xbehave.Sdk.Specifications.Net40.dll" },
  { :command => xunit_command_net40, :assembly => "src/test/Xbehave.Specifications.Net40/bin/Debug/Xbehave.Specifications.Net40.dll" }
]

features = [
  { :command => xunit_command_net20, :assembly => "src/test/Xbehave.Features.Net35/bin/Debug/Xbehave.Features.Net35.dll" },
  { :command => xunit_command_net40, :assembly => "src/test/Xbehave.Features.Net40/bin/Debug/Xbehave.Features.Net40.dll" }
]

samples = [
  { :command => xunit_command_net20, :assembly => "src/Xbehave.Samples.Net35/bin/Debug/Xbehave.Samples.Net35.dll" },
  { :command => xunit_command_net40, :assembly => "src/Xbehave.Samples.Net40/bin/Debug/Xbehave.Samples.Net40.dll" }
]

nuspec = "src/Xbehave.nuspec"

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Execute default tasks"
task :default => [:spec, :feature, :pack]

desc "Clean solution"
msbuild :clean do |msb|
  FileUtils.rmtree output
  msb.properties = { :configuration => :Release }
  msb.targets = [:Clean]
  msb.solution = solution
end

desc "Build solution"
msbuild :build => [:clean] do |msb|
  msb.properties = { :configuration => :Release }
  msb.targets = [:Build]
  msb.solution = solution
end

desc "Execute specs"
task :spec => [:build] do
  execute_xunit specs
end

desc "Execute features"
task :feature => [:build] do
  execute_xunit features
end

desc "Create the nuget package"
nugetpack :pack => [:build] do |nuget|
  FileUtils.mkpath output
  nuget.command = nuget_command
  nuget.nuspec = nuspec
  nuget.output = output
end

desc "Execute samples"
task :sample => [:build] do
  execute_xunit samples
end

def execute_xunit(tests)
  tests.each do |test|
    xunit = XUnitTestRunner.new
    xunit.command = test[:command]
    xunit.assembly = test[:assembly]
    xunit.options "/html", test[:assembly] + ".TestResults.html", "/xml", test[:assembly] + ".TestResults.xml"
    xunit.execute  
  end
end
