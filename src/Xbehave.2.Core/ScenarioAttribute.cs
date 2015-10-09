// <copyright file="ScenarioAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate the definition of a scenario.
    /// A scenario can also be fed examples from a data source, mapping to parameters on the scenario method.
    /// If the data source contains multiple rows,
    /// then the scenario method is executed multiple times (once with each data row).
    /// Examples can be fed to the scenario by applying one or more instances of <see cref="ExampleAttribute"/>
    /// or any other attribute inheriting from <see cref="Xunit.Sdk.DataAttribute"/>.
    /// E.g. <see cref="Xunit.InlineDataAttribute"/> or
    /// <see cref="Xunit.MemberDataAttribute"/>.
    /// </summary>
#if PLATFORM_DNX
    [XunitTestCaseDiscoverer("Xbehave.Execution.ScenarioDiscoverer", "xunit.execution.xbehave")]
#else
    [XunitTestCaseDiscoverer("Xbehave.Execution.ScenarioDiscoverer", "Xbehave.Execution.{Platform}")]
#endif
    [AttributeUsage(AttributeTargets.Method)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class ScenarioAttribute : FactAttribute
    {
    }
}
