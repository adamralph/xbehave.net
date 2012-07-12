// <copyright file="BackgroundAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate a background for each scenario defined in the same feature class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class BackgroundAttribute : Attribute
    {
        /// <summary>
        /// Enumerates the commands representing the backgrounds defined by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The test method</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the backgrounds defined by the <paramref name="method"/>.</returns>
        /// <remarks>This method may be overridden.</remarks>
        protected internal virtual IEnumerable<ITestCommand> EnumerateBackgroundCommands(IMethodInfo method)
        {
            yield return new FactCommand(method);
        }
    }
}
