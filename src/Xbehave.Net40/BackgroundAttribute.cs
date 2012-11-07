// <copyright file="BackgroundAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Sdk;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate a background for each scenario defined in the same feature class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class BackgroundAttribute : Attribute
    {
        /// <summary>
        /// Creates the commands representing the backgrounds defined by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The test method.</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the backgrounds defined by the <paramref name="method"/>.</returns>
        public IEnumerable<ITestCommand> CreateBackgroundCommands(IMethodInfo method)
        {
            return this.EnumerateBackgroundCommands(method);
        }

        /// <summary>
        /// Enumerates the commands representing the backgrounds defined by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The test method.</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the backgrounds defined by the <paramref name="method"/>.</returns>
        protected virtual IEnumerable<ITestCommand> EnumerateBackgroundCommands(IMethodInfo method)
        {
            yield return new BackgroundCommand(method);
        }
    }
}
