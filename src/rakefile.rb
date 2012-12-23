require 'rubygems'
require 'albacore'
require 'fileutils'

task :default => [ :clean, :build, :spec, :features ]

desc "Execute features"
xunit :features do |xunit|
  FileUtils.rmtree "bin/features"
  FileUtils.mkpath "bin/features"
  xunit.command = "../packages/xunit.runners.1.9.1/tools/xunit.console.clr4.exe"
  xunit.assemblies = [ "test/Xbehave.Test.Acceptance.Net35/bin/Debug/Xbehave.Test.Acceptance.Net35.dll", "test/Xbehave.Test.Acceptance.Net40/bin/Debug/Xbehave.Test.Acceptance.Net40.dll" ]
  xunit.html_output = "bin/features"
end

desc "Execute specs"
xunit :spec do |xunit|
  FileUtils.rmtree "bin/spec"
  FileUtils.mkpath "bin/spec"
  xunit.command = "../packages/xunit.runners.1.9.1/tools/xunit.console.clr4.exe"
  xunit.assemblies = [ "test/Xbehave.Sdk.Test.Unit.Net35/bin/Debug/Xbehave.Sdk.Test.Unit.Net35.dll", "test/Xbehave.Test.Unit.Net35/bin/Debug/Xbehave.Test.Unit.Net35.dll", "test/Xbehave.Sdk.Test.Unit.Net40/bin/Debug/Xbehave.Sdk.Test.Unit.Net40.dll", "test/Xbehave.Test.Unit.Net40/bin/Debug/Xbehave.Test.Unit.Net40.dll" ]
  xunit.html_output = "bin/spec"
end

desc "Build solution"
msbuild :build do |msb|
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Build ]
  msb.solution = "XBehave.sln"
end

desc "Clean solution and build artifacts"
msbuild :clean do |msb|
  FileUtils.rmtree "bin"

  msb.properties = { :configuration => :Release }
  msb.targets = [ :Clean ]
  msb.solution = "XBehave.sln"
end