// <copyright file="ThesisAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using Xunit.Extensions;
    using Xunit.Sdk;

    /// <summary>
    /// This member is deprecated.
    /// </summary>
    [Obsolete("Use ScenarioAttribute instead.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ThesisAttribute : TheoryAttribute
    {
        /// <summary>
        /// Creates instances of <see cref="T:Xunit.Extensions.TheoryCommand"/> which represent individual intended
        /// invocations of the test method, one per data row in the data source.
        /// </summary>
        /// <param name="method">The method under test</param>
        /// <returns>
        /// An enumerator through the desired test method invocations
        /// </returns>
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            var theoryTestCommands = base.EnumerateTestCommands(method);
            return ScenarioAttribute.GetTheoryCommands(method, theoryTestCommands);
        }
    }
}
