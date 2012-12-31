require 'albacore'
require 'fileutils'
require File.expand_path('rakehelper/rakehelper', File.dirname(__FILE__))

ENV["XunitConsole_net20"] = "../packages/xunit.runners.1.9.1/tools/xunit.console.exe"
ENV["XunitConsole_net40"] = "../packages/xunit.runners.1.9.1/tools/xunit.console.clr4.exe"
ENV["NuGetConsole"] = "../packages/NuGet.CommandLine.2.1.2/tools/NuGet.exe"

Albacore.configure do |config|
  config.log_level = :verbose
end

desc "Executes clean, build, spec, feature and nugetpack"
task :default => [ :clean, :build, :spec, :feature, :nugetpack ]

desc "Clean solution"
task :clean do
  FileUtils.rmtree "bin"

  build = RakeHelper.use_mono ? XBuild.new : MSBuild.new
  build.properties = { :configuration => :Release }
  build.targets = [ :Clean ]
  build.solution = "XBehave.sln"
  build.execute
end

desc "Build solution"
task :build do
  build = RakeHelper.use_mono ? XBuild.new : MSBuild.new
  build.properties = { :configuration => :Release }
  build.targets = [ :Build ]
  build.solution = "XBehave.sln"
  build.execute
end

desc "Execute specs"
task :spec do
  specs = [
    { :version => :net20, :path => "test/Xbehave.Sdk.Test.Unit.Net35/bin/Debug/Xbehave.Sdk.Test.Unit.Net35.dll" },
    { :version => :net20, :path => "test/Xbehave.Test.Unit.Net35/bin/Debug/Xbehave.Test.Unit.Net35.dll" },
    { :version => :net40, :path => "test/Xbehave.Sdk.Test.Unit.Net40/bin/Debug/Xbehave.Sdk.Test.Unit.Net40.dll" },
    { :version => :net40, :path => "test/Xbehave.Test.Unit.Net40/bin/Debug/Xbehave.Test.Unit.Net40.dll" }
  ]
  execute specs
end

desc "Execute features"
task :feature do
  features = [
    { :version => :net20, :path => "test/Xbehave.Test.Acceptance.Net35/bin/Debug/Xbehave.Test.Acceptance.Net35.dll" },
    { :version => :net40, :path => "test/Xbehave.Test.Acceptance.Net40/bin/Debug/Xbehave.Test.Acceptance.Net40.dll" }
  ]
  execute features
end

desc "Create the nuget package"
nugetpack :nugetpack do |nuget|
  FileUtils.mkpath "bin"
  
  # NOTE (Adam): nuspec files can be consolidated after NuGet 2.3 is released - see http://nuget.codeplex.com/workitem/2767
  nuget.command = RakeHelper.nuget_command
  nuget.nuspec = [ "Xbehave", ENV["OS"], "nuspec" ].select { |token| token }.join(".")  
  nuget.output = "bin"
end

desc "Execute samples"
task :sample do
  samples = [
    { :version => :net20, :path => "Xbehave.Samples.Net35/bin/Debug/Xbehave.Samples.Net35.dll" },
    { :version => :net40, :path => "Xbehave.Samples.Net40/bin/Debug/Xbehave.Samples.Net40.dll" }
  ]
  execute samples
end

def execute(tests)
  tests.each do |test|
    xunit = XUnitTestRunner.new
    xunit.command = RakeHelper.xunit_command(test[:version])
    xunit.assembly = test[:path]
    xunit.options "/html " + test[:path] + ".html"
    xunit.execute  
  end
end
