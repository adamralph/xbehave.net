// <copyright file="BackgroundAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate a background that should be run by the test runner, once for each scenario defined as part of the feature.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class BackgroundAttribute : FactAttribute
    {
        /// <summary>
        /// Enumerates the test commands represented by this background method which will register the background steps defined by the method.
        /// Derived classes should override this method to return instances of <see cref="T:Xunit.Sdk.ITestCommand"/>.
        /// Normally only one instance should be returned, which represents the registration of the background steps defined by the method.
        /// </summary>
        /// <param name="method">The test method</param>
        /// <returns>The test commands which will register the background steps defined by the given method.</returns>
        public virtual IEnumerable<ITestCommand> CreateBackgroundCommands(IMethodInfo method)
        {
            return base.EnumerateTestCommands(method);
        }

        /// <summary>
        /// Overrides the behaviour of the base class to return no test commands.
        /// Derived classes should not override this method.
        /// </summary>
        /// <param name="method">The test method</param>
        /// <returns>No instances of <see cref="T:Xunit.Sdk.ITestCommand"/>.</returns>
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            yield break;
        }
    }
}
