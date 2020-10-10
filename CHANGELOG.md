# Changelog

## 2.4.1

### Enhancements

- [#617: **[BREAKING]** Throw an exception when steps are defined which will not be executed](https://github.com/adamralph/xbehave.net/issues/617)

## 2.4.0

### Enhancements

- [#155: DateTime, DateTimeOffset, and Guid values in Examples](https://github.com/adamralph/xbehave.net/issues/155)
- [#423: Link against xunit 2.4](https://github.com/adamralph/xbehave.net/issues/423)
- [#428: upgrade from SourceLink 2.5.0 to 2.8.0](https://github.com/adamralph/xbehave.net/pull/428)

### Other

- [#386: **[BREAKING]** Remove f() and \_()](https://github.com/adamralph/xbehave.net/issues/386)

## 2.3.1

### Enhancements

- [#411: Step display text customisation](https://github.com/adamralph/xbehave.net/issues/411)

### Fixed bugs

- [#410: Background steps showing as dummy inconclusive tests in ReSharper](https://github.com/adamralph/xbehave.net/issues/410)

## 2.3.0

### Enhancements

- [#186: Introduce a BeforeAfterScenarioAttribute](https://github.com/adamralph/xbehave.net/issues/186)
- [#196: **[BREAKING]** Async teardowns](https://github.com/adamralph/xbehave.net/issues/196)
- [#263: **[BREAKING]** Step context for teardowns](https://github.com/adamralph/xbehave.net/issues/263)
- [#298: Source stepping](https://github.com/adamralph/xbehave.net/issues/298)
- [#300: Support .NET Standard 1.1 (including .NET Core 1.0 onwards)](https://github.com/adamralph/xbehave.net/issues/300)
- [#371: Consistent package id's and titles](https://github.com/adamralph/xbehave.net/issues/371)
- [#375: **[BREAKING]** Link against xUnit.net 2.3.x](https://github.com/adamralph/xbehave.net/issues/375)
- [#388: Use improved argument formatter from xunit 2.3](https://github.com/adamralph/xbehave.net/issues/388)
- [#397: Support example skipping](https://github.com/adamralph/xbehave.net/issues/397)

### Other

- [#391: **[BREAKING]** Remove injection of example values into step names](https://github.com/adamralph/xbehave.net/issues/391)

## 2.1.4

### Fixed bugs

- [#342: AppDomainUnloadedException when test runner exits](https://github.com/adamralph/xbehave.net/issues/342)

## 2.1.3

### Fixed bugs

- [#329: ITestOutputHelper does not work in xbehave steps](https://github.com/adamralph/xbehave.net/issues/329)

## 2.1.0

### Enhancements

- [#267: Render array elements in test output](https://github.com/adamralph/xbehave.net/issues/267)

## 2.0.1

### Fixed bugs

- [#271: Scenarios are not run when using xunit 2.1](https://github.com/adamralph/xbehave.net/issues/271)

## 2.0.0

### Enhancements

- [#51: **[BREAKING]** Switch to xunit 2.0](https://github.com/adamralph/xbehave.net/issues/51)
- [#146: IStepContext metadata](https://github.com/adamralph/xbehave.net/issues/146)
- [#238: 'x' method](https://github.com/adamralph/xbehave.net/issues/238)
- [#245: Steps which follow a failed a step are skipped](https://github.com/adamralph/xbehave.net/issues/245)
- [#247: OnFailure](https://github.com/adamralph/xbehave.net/issues/247)
- [#254: StepDefinition filter attributes](https://github.com/adamralph/xbehave.net/issues/254)

## 1.1.0

### Enhancements

- [#25: Support async steps](https://github.com/adamralph/xbehave.net/issues/25)

### Fixed bugs

- [#135: Objects not being disposed when a step has a timeout](https://github.com/adamralph/xbehave.net/issues/135)

## 1.0.0

## 0.17.0

## 0.16.0

### Enhancements

- [#88: Upgrade to xUnit.net 1.9.2](https://github.com/adamralph/xbehave.net/issues/88)

## 0.15.0

### Enhancements

- [#37: Optional omission of arguments from scenario names in test output](https://github.com/adamralph/xbehave.net/issues/37)
- [#57: Add optional continuation on failure after the execution of specific step types](https://github.com/adamralph/xbehave.net/issues/57)
- [#63: Add f() extension method](https://github.com/adamralph/xbehave.net/issues/63)
- [#67: Rename X() to f()](https://github.com/adamralph/xbehave.net/issues/67)
- [#72: Add an underscore string extension method](https://github.com/adamralph/xbehave.net/issues/72)

### Fixed bugs

- [#28: Null values are not rendered in step text](https://github.com/adamralph/xbehave.net/issues/28)

### Other

- [#4: **[BREAKING]** Remove expression overloads](https://github.com/adamralph/xbehave.net/issues/4)
- [#13: **[BREAKING]** Remove all deprecated methods](https://github.com/adamralph/xbehave.net/issues/13)

## 0.14.0

### Enhancements

- [#5: Pass default values for scenario parameters not covered by examples](https://github.com/adamralph/xbehave.net/issues/5)

### Fixed bugs

- [#7: Scenario methods with malformed parameters and examples fail silently](https://github.com/adamralph/xbehave.net/issues/7)
