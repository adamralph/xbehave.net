using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace Xbehave
{
    /// <summary>
    /// Provides example values for a scenario passed as arguments to the scenario method.
    /// This attribute is designed as a synonym of <see cref="Xunit.InlineDataAttribute"/>,
    /// which is the most commonly used data attribute, but you can also use any type of attribute derived from
    /// <see cref="Xunit.Sdk.DataAttribute"/> to provide a data source for a scenario.
    /// E.g. <see cref="Xunit.InlineDataAttribute"/> or
    /// <see cref="Xunit.MemberDataAttribute"/>.
    /// </summary>
    [DataDiscoverer("Xunit.Sdk.InlineDataDiscoverer", "xunit.core")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ExampleAttribute : DataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleAttribute"/> class.
        /// This attribute is designed as a synonym of <see cref="Xunit.InlineDataAttribute"/>,
        /// which is the most commonly used data attribute, but you can also use any type of attribute derived from
        /// <see cref="Xunit.Sdk.DataAttribute"/> to provide a data source for a scenario.
        /// E.g. <see cref="Xunit.InlineDataAttribute"/> or
        /// <see cref="Xunit.MemberDataAttribute"/>.
        /// </summary>
        /// <param name="data">The data values to pass to the scenario.</param>
        public ExampleAttribute(params object[] data)
        {
        }

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod) => throw new InvalidOperationException();
    }
}
