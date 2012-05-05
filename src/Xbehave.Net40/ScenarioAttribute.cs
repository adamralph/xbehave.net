// <copyright file="ScenarioAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xbehave.Infra;
    using Xbehave.Internal;
    using Xunit.Extensions;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate a scenario that should be run by the test runner.
    /// A scenario can also be fed from a data source, mapping to parameters on the scenario method.
    /// If the data source contains multiple rows, then the scenario method is executed multiple times (once with each data row).
    /// Data can be fed to the scenario by applying one or more instances of <see cref="ScenarioDataAttribute"/>
    /// or any other attribute inheriting from <see cref="Xunit.Extensions.DataAttribute"/>.
    /// E.g. <see cref="Xunit.Extensions.ClassDataAttribute"/>,
    /// <see cref="Xunit.Extensions.OleDbDataAttribute"/>,
    /// <see cref="Xunit.Extensions.SqlServerDataAttribute"/>,
    /// <see cref="Xunit.Extensions.ExcelDataAttribute"/> or
    /// <see cref="Xunit.Extensions.PropertyDataAttribute"/>.
    /// </summary>    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class ScenarioAttribute : TheoryAttribute
    {
        /// <summary>
        /// Enumerates the test commands represented by this test method.
        /// Derived classes should override this method to return instances of <see cref="T:Xunit.Sdk.ITestCommand"/>, one per execution of a test method.
        /// </summary>
        /// <param name="method">The test method</param>
        /// <returns>The test commands which will execute the test runs for the given method.</returns>
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            Require.NotNull(method, "method");

            foreach (var xunitCommand in this.CreateXunitCommands(method))
            {
                foreach (var stepCommand in CreateStepCommands(method, xunitCommand))
                {
                    yield return stepCommand;
                }
            }
        }

        private static IEnumerable<ITestCommand> CreateStepCommands(IMethodInfo method, ITestCommand xunitCommand)
        {
            var theoryCommand = xunitCommand as TheoryCommand;
            var arguments = theoryCommand == null ? null : theoryCommand.Parameters;
            var testClassInstance = method.IsStatic ? null : method.CreateInstance();
            return ThreadContext.CreateTestCommands(new MethodCall(method, arguments), () => xunitCommand.Execute(testClassInstance));
        }

        private IEnumerable<ITestCommand> CreateXunitCommands(IMethodInfo method)
        {
            return method.MethodInfo != null && method.MethodInfo.GetParameters().Any()
                ? base.EnumerateTestCommands(method)
                : new[] { new FactCommand(method) };
        }
    }
}
