// <copyright file="ExampleAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Provides example values for a scenario passed as arguments to the scenario method.
    /// This attribute is designed as a synonym of <see cref="Xunit.InlineDataAttribute"/>,
    /// which is the most commonly used data attribute, but you can also use any type of attribute derived from
    /// <see cref="Xunit.Sdk.DataAttribute"/> to provide a data source for a scenario.
    /// E.g. <see cref="Xunit.InlineDataAttribute"/> or
    /// <see cref="Xunit.MemberDataAttribute"/>.
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class ExampleAttribute : DataAttribute
    {
        private readonly InlineDataAttribute wrappee;

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
            this.wrappee = new InlineDataAttribute(data);
        }

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return this.wrappee.GetData(testMethod);
        }
    }
}
