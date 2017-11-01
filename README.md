<img src="assets/xbehave_256x256.png" width="128" />

[![NuGet Badge](https://buildstats.info/nuget/Xbehave)](https://www.nuget.org/packages/Xbehave/)
[![Build status](https://ci.appveyor.com/api/projects/status/2hs60yhjdoucwu7i/branch/dev?svg=true)](https://ci.appveyor.com/project/adamralph/xbehave-net/branch/dev)
[![Source Browser](https://img.shields.io/badge/Browse-Source-green.svg)](http://sourcebrowser.io/Browse/xbehave/xbehave.net)

[![Join the chat at https://gitter.im/xbehave/xbehave.net](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/xbehave/xbehave.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Follow @xbehavenet](https://img.shields.io/badge/Twitter-Follow%20%40xbehavenet-blue.svg)](https://twitter.com/intent/follow?screen_name=xbehavenet)

* [Website](http://xbehave.github.io/)
* [Quickstart](https://github.com/xbehave/xbehave.net/wiki/Quickstart)
* [Documentation](https://github.com/xbehave/xbehave.net/wiki)
* [FAQ](https://github.com/xbehave/xbehave.net/wiki/FAQ)

## Where can I get it?

xBehave.net is available as a [NuGet package](https://nuget.org/packages/xBehave). For update notifications, follow [@xbehavenet](https://twitter.com/#!/xbehavenet). CI builds are available at [AppVeyor](https://ci.appveyor.com/project/adamralph/xbehave-net). To build manually, clone or fork this repository and see ['How to build'](#how-to-build).

## Can I help to improve it and/or fix bugs?

Absolutely! Please feel free to raise issues, fork the source code, send pull requests, etc. **No pull request is too small.** Even trivial white space fixes are appreciated. For more details see [CONTRIBUTING.md](/CONTRIBUTING.md).

## How to build

The only prerequisites you need are [.NET CLI](https://github.com/dotnet/cli) and .NET Framework 4.5.

Navigate to your clone root folder and execute `dotnet restore` followed by `build.cmd`.

`build.cmd` executes the default build targets which include compilation, test execution and packaging. After the build has completed, the build artifacts will be located in `artifacts/`.

For full usage details for `build.cmd`, execute `build.cmd -?`. See  [simple-targets-csx](https://github.com/adamralph/simple-targets-csx) for more info.

You can also build the solution using Visual Studio 2015 or later. At the time of writing the build is only confirmed to work on Windows.

## Versions

xBehave.net follows the versioning scheme of xUnit.net. xUnit.net and xBehave.net do not follow SemVer and may introduce breaking changes in minor versions. Each minor version of xBehave.net is linked to the equivalent minor version of xUnit.net. xBehave.net patch versions within a minor version are independent of patch versions of xUnit.net within that minor version. For example, xBehave.net 2.3.x is linked to xUnit.net 2.3.x, but each package may have a different number of patch versions within the 2.3.x range.

## On which giants' shoulders does it stand?

* [.NET CLI](https://github.com/dotnet/cli)
* [AppVeyor](https://ci.appveyor.com/project/adamralph/xbehave-net/)
* [Dan North](http://dannorth.net/introducing-bdd/)
* [EditorConfig](http://editorconfig.org/)
* [FluentAssertions](http://www.fluentassertions.com/)
* [Gherkin](https://github.com/cucumber/cucumber/wiki/Gherkin/)
* [GitHub](https://github.com/xbehave/xbehave.net/)
* [Gitter](https://gitter.im/xbehave/xbehave.net/)
* [LiteGuard](https://github.com/liteguard/liteguard/)
* [NuGet](https://www.nuget.org/packages/Xbehave/)
* [simple-targets-csx](https://github.com/adamralph/simple-targets-csx)
* [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/)
* [SubSpec](http://bitbucket.org/johannesrudolph/subspec/)
* [xUnit.net](https://xunit.github.io/)

---

<sub>xBehave.net logo designed by [Vanja Pakaski](https://github.com/vanpak).</sub>
