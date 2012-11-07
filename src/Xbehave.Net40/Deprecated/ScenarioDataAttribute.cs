// <copyright file="ScenarioDataAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit.Extensions;

    /// <summary>
    /// Provides a data source for a scenario, with the data coming from inline values.
    /// This attribute is designed as a synonym of <see cref="Xunit.Extensions.InlineDataAttribute"/>,
    /// which is the most commonly used data attribute, but you can also use any type of attribute derived from
    /// <see cref="Xunit.Extensions.DataAttribute"/> to provide a data source for a scenario.
    /// E.g. <see cref="Xunit.Extensions.ClassDataAttribute"/>,
    /// <see cref="Xunit.Extensions.OleDbDataAttribute"/>,
    /// <see cref="Xunit.Extensions.SqlServerDataAttribute"/>,
    /// <see cref="Xunit.Extensions.ExcelDataAttribute"/> or
    /// <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
    /// </summary>
    /// <remarks>Deprecated in favor of ExampleAttribute in version 0.10.0.</remarks>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    [Obsolete("Use the Example attribute instead.")]
    public class ScenarioDataAttribute : InlineDataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioDataAttribute"/> class.
        /// This attribute is designed as a synonym of <see cref="Xunit.Extensions.InlineDataAttribute"/>,
        /// which is the most commonly used data attribute, but you can also use any type of attribute derived from
        /// <see cref="Xunit.Extensions.DataAttribute"/> to provide a data source for a scenario.
        /// E.g. <see cref="Xunit.Extensions.ClassDataAttribute"/>,
        /// <see cref="Xunit.Extensions.OleDbDataAttribute"/>,
        /// <see cref="Xunit.Extensions.SqlServerDataAttribute"/>,
        /// <see cref="Xunit.Extensions.ExcelDataAttribute"/> or
        /// <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
        /// </summary>
        /// <param name="dataValues">The data values to pass to the scenario.</param>
        public ScenarioDataAttribute(params object[] dataValues)
            : base(dataValues)
        {
        }
    }
}
