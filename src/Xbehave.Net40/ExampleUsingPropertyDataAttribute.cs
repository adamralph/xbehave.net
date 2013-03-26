// <copyright file="ExampleUsingPropertyDataAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit.Extensions;

    /// <summary>
    /// Provides a data source for an ExamplePropertyData, with the data coming from a public static property on the test class.
    /// The property must return IEnumerable&lt;object[]&gt; with the test data.
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class ExampleUsingPropertyDataAttribute : PropertyDataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleUsingPropertyDataAttribute"/> class.
        /// This attribute is designed as a synonym of <see cref="Xunit.Extensions.PropertyDataAttribute"/>.PropertyDataAttribute
        /// You can also use any type of attribute derived from
        /// <see cref="Xunit.Extensions.PropertyDataAttribute"/> to provide a data source for a scenario.
        /// E.g. <see cref="Xunit.Extensions.ClassDataAttribute"/>,
        /// <see cref="Xunit.Extensions.OleDbDataAttribute"/>,
        /// <see cref="Xunit.Extensions.SqlServerDataAttribute"/>,
        /// <see cref="Xunit.Extensions.ExcelDataAttribute"/> or
        /// <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
        /// </summary>
        /// <param name="propertyName">The data values to pass to the scenario.</param>
        public ExampleUsingPropertyDataAttribute(string propertyName)
            : base(propertyName)
        {
        }
    }
}