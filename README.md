![xBehave.net](/assets/xbehave_128x128.png)

[![NuGet Badge](https://buildstats.info/nuget/Xbehave)](https://www.nuget.org/packages/Xbehave/)
[![Join the chat at https://gitter.im/xbehave/xbehave.net](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/xbehave/xbehave.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/2hs60yhjdoucwu7i/branch/dev?svg=true)](https://ci.appveyor.com/project/adamralph/xbehave-net/branch/dev)

* [Website](http://xbehave.github.io/)
* [Quickstart](https://github.com/xbehave/xbehave.net/wiki/Quickstart)
* [Documentation](https://github.com/xbehave/xbehave.net/wiki)
* [FAQ](https://github.com/xbehave/xbehave.net/wiki/FAQ)

## Where can I get it?

xBehave.net is available as a [NuGet package](https://nuget.org/packages/xBehave). For update notifications, follow [@adamralph](https://twitter.com/#!/adamralph). CI builds are available at [AppVeyor](https://ci.appveyor.com/project/adamralph/xbehave-net). To build manually, clone or fork this repository and see ['How to build'](#how-to-build).

## Can I help to improve it and/or fix bugs?

Absolutely! Please feel free to raise issues, fork the source code, send pull requests, etc. **No pull request is too small.** Even trivial white space fixes are appreciated. For more details see [CONTRIBUTING.md](/CONTRIBUTING.md).

## How to build

Navigate to your clone root folder and execute `build.cmd`. The only prequisite you need is MSBuild 14, which is also included in Visual Studio 2015.

`build.cmd` executes the default build targets which include compilation, test execution and packaging. After the build has completed, the build artifacts will be located in `artifacts/output/`.

You can also build the solution using Visual Studio 2015 or later. At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

### Extras

* View the full list of build targets:

    `build.cmd -T`

* Run a specific target:

    `build.cmd build`

* Run multiple targets:

    `build.cmd build pack`

* View the full list of options:

    `build.cmd -?`

## On which giant's shoulders does it stand?

* [xUnit.net](https://xunit.github.io/)
* [Dan North](http://dannorth.net/introducing-bdd/)
* [Gherkin](https://github.com/cucumber/cucumber/wiki/Gherkin/)
* [SubSpec](http://bitbucket.org/johannesrudolph/subspec/)
* [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/)
* [FluentAssertions](http://www.fluentassertions.com/)
* [LiteGuard](https://github.com/liteguard/liteguard/)
* [Semantic Versioning](http://semver.org/)
* [AppVeyor](https://ci.appveyor.com/project/adamralph/xbehave-net/)
* [NuGet](https://www.nuget.org/packages/Xbehave/)

xBehave.net logo designed by [Vanja Pakaski](https://github.com/vanpak).
