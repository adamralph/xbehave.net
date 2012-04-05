// <copyright file="SpecificationAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// This member is deprecated (was part of the original SubSpec API).
    /// </summary>
    [Obsolete("Use ScenarioAttribute instead.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SpecificationAttribute : FactAttribute
    {
        /// <summary>
        /// Enumerates the test commands represented by this test method. Derived classes should
        /// override this method to return instances of <see cref="T:Xunit.Sdk.ITestCommand"/>, one per execution
        /// of a test method.
        /// </summary>
        /// <param name="method">The test method</param>
        /// <returns>The test commands which will execute the test runs for the given method.</returns>
        public static IEnumerable<ITestCommand> FtoEnumerateTestCommands(IMethodInfo method)
        {
            return ScenarioAttribute.GetFactCommands(method);
        }

        /// <summary>
        /// Enumerates the test commands represented by this test method. Derived classes should
        /// override this method to return instances of <see cref="T:Xunit.Sdk.ITestCommand"/>, one per execution
        /// of a test method.
        /// </summary>
        /// <param name="method">The test method</param>
        /// <returns>The test commands which will execute the test runs for the given method.</returns>
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            return ScenarioAttribute.GetFactCommands(method);
        }
    }
}
