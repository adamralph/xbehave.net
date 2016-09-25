![xBehave.net](/assets/xbehave_128x128.png)

[![NuGet Badge](https://buildstats.info/nuget/Xbehave)](https://www.nuget.org/packages/Xbehave/)
[![Join the chat at https://gitter.im/xbehave/xbehave.net](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/xbehave/xbehave.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/2hs60yhjdoucwu7i/branch/dev?svg=true)](https://ci.appveyor.com/project/adamralph/xbehave-net/branch/dev)

* [Website](http://xbehave.github.io/)
* [Quickstart](https://github.com/xbehave/xbehave.net/wiki/Quickstart)
* [Documentation](https://github.com/xbehave/xbehave.net/wiki)
* [FAQ](https://github.com/xbehave/xbehave.net/wiki/FAQ)

## Where can I get it? ##

xBehave.net is available as a [NuGet package](https://nuget.org/packages/xBehave). For update notifications, follow [@adamralph](https://twitter.com/#!/adamralph).

CI builds are available at [AppVeyor](https://ci.appveyor.com/project/adamralph/xbehave-net). To build manually, clone or fork this repository and see ['How to build'](#how-to-build).

## Can I help to improve it and/or fix bugs? ##

Absolutely! Please feel free to raise issues, fork the source code, send pull requests, etc.

**No pull request is too small.** Even trivial white space fixes are appreciated.

Before you contribute anything make sure you read [CONTRIBUTING.md](/CONTRIBUTING.md).

## How to build

These instructions are *only* for building with Rake, which includes compilation, test execution and packaging. This is the simplest way to build.

You can also build the solution using Visual Studio 2012 or later.

*Don't be put off by the prerequisites!* It only takes a few minutes to set them up and only needs to be done once. If you haven't used [Rake](http://rake.rubyforge.org/ "RAKE -- Ruby Make") before then you're in for a real treat!

At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

### Prerequisites

1. Ensure you have .NET framework 3.5 and 4.5 installed.

1. Install Ruby 1.8.7 or later.

 For Windows we recommend using [RubyInstaller](http://rubyinstaller.org/) and selecting 'Add Ruby executables to your PATH' when prompted. For alternatives see the [Ruby download page](http://www.ruby-lang.org/en/downloads/).
1. Using a command prompt, update RubyGems to the latest version:

    `gem update --system`

1. Install Bundler:

    `gem install bundler`

1. Install the required gems:

    `bundle install`

### Building

Using a command prompt, navigate to your clone root folder and execute:

`build.cmd`

This executes the default build tasks. After the build has completed, the build artifacts will be located in `artifacts/output/`.

### Extras

* View the full list of build tasks:

    `build.cmd -T`

* Run a specific task:

    `build.cmd spec`

* Run multiple tasks:

    `build.cmd spec pack`

* View the full list of rake options:

    `build.cmd -h`

## On which giant's shoulders does it stand?

* [xUnit.net](https://xunit.github.io/)
* [Dan North](http://dannorth.net/introducing-bdd/)
* [Gherkin](https://github.com/cucumber/cucumber/wiki/Gherkin/)
* [SubSpec](http://bitbucket.org/johannesrudolph/subspec/)
* [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/)
* [Rake](http://rake.rubyforge.org/)
* [FluentAssertions](http://www.fluentassertions.com/)
* [LiteGuard](https://github.com/liteguard/liteguard/)
* [Semantic Versioning](http://semver.org/)
* [AppVeyor](https://ci.appveyor.com/project/adamralph/xbehave-net/)
* [NuGet](https://www.nuget.org/packages/Xbehave/)

xBehave.net logo designed by [Vanja Pakaski](https://github.com/vanpak).
