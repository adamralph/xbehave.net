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
    using Xunit.Extensions;
    using Xunit.Sdk;
    using Guard = Xbehave.Infra.Guard;

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
            IEnumerable<ITestCommand> scenarioCommands;
            object feature;

            // NOTE: any exception must be wrapped in a command, otherwise the test runner will simply retry this method infinitely
            try
            {
                if (method == null)
                {
                    throw new ArgumentNullException("method");
                }

                if (method.MethodInfo == null)
                {
                    throw new ArgumentException("method.MethodInfo is null.", "method");
                }

                scenarioCommands = method.MethodInfo.GetParameters().Any()
                    ? base.EnumerateTestCommands(method).ToArray() // NOTE: current impl does not yield but we enumerate now to be future proof
                    : new[] { new TheoryCommand(method, new object[0]) };

                feature = method.IsStatic ? null : method.CreateInstance();
            }
            catch (Exception ex)
            {
                return new[] { new ExceptionCommand(method, ex) };
            }

            // NOTE: this is not in the try catch since we are yielding internally
            // TODO: address this - see http://stackoverflow.com/a/346772/49241
            return scenarioCommands.Where(scenarioCommand => !(scenarioCommand is TheoryCommand)).Concat(
                scenarioCommands.OfType<TheoryCommand>().SelectMany(scenarioCommand =>
                 CurrentScenario.CreateCommands(new ScenarioDefinition(method, scenarioCommand.Parameters, () => scenarioCommand.Execute(feature)))));
        }
    }
}
