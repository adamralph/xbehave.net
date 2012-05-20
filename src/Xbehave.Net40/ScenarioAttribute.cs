// <copyright file="ScenarioAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xbehave.Sdk;
    using Xbehave.Sdk.Infra;
    using Xunit.Extensions;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate a scenario that should be run by the test runner.
    /// A scenario can also be fed examples from a data source, mapping to parameters on the scenario method.
    /// If the data source contains multiple rows, then the scenario method is executed multiple times (once with each data row).
    /// Examples can be fed to the scenario by applying one or more instances of <see cref="ExampleAttribute"/>
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

            var scenarioDefinitions = method.MethodInfo != null && method.MethodInfo.GetParameters().Any()
                ? base.EnumerateTestCommands(method)
                : new[] { new FactCommand(method) };

            return scenarioDefinitions.SelectMany(definition => CreateCommands(method, definition));
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Required to prevent infinite loops in test runners (TestDrive.NET, Resharper) when they are allowed to handle exceptions.")]
        private static IEnumerable<ITestCommand> CreateCommands(IMethodInfo method, ITestCommand scenarioDefinition)
        {
            var feature = method.IsStatic ? null : method.CreateInstance();

            var theoryCommand = scenarioDefinition as TheoryCommand;
            var call = new MethodCall(method, theoryCommand == null ? new object[0] : theoryCommand.Parameters);

            // NOTE: I've tried to move this into Scenario, with the finally block clearing the steps but it just doesn't seem to work
            try
            {
                try
                {
                    scenarioDefinition.Execute(feature);
                }
                catch (Exception ex)
                {
                    return new ITestCommand[] { new ExceptionCommand(method, ex) };
                }
                
                return CurrentThread.Scenario.GetTestCommands(call);
            }
            finally
            {
                CurrentThread.ResetScenario();
            }
        }
    }
}
