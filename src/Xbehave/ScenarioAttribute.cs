// <copyright file="ScenarioAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

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
        /// Initializes a new instance of the <see cref="ScenarioAttribute"/> class.
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
        public ScenarioAttribute()
        {
        }

        internal static IEnumerable<ITestCommand> GetFactCommands(IMethodInfo method)
        {
            return ScenarioContext.SafelyEnumerateTestCommands(method, RegisterSteps);
        }

        internal static IEnumerable<ITestCommand> GetTheoryCommands(IMethodInfo method, IEnumerable<ITestCommand> theoryTestCommands)
        {
            var commands = new List<ITestCommand>();
            foreach (var command in theoryTestCommands)
            {
                if (command is TheoryCommand)
                {
                    var itemCommands = ScenarioContext.SafelyEnumerateTestCommands(
                        method, m => command.Execute(command.ShouldCreateInstance ? Activator.CreateInstance(method.MethodInfo.ReflectedType) : null));

                    commands.AddRange(itemCommands);
                }
                else
                {
                    commands.Clear();
                    commands.Add(command);
                    break;
                }
            }

            return commands;
        }

        /// <summary>
        /// Enumerates the test commands represented by this test method.
        /// Derived classes should override this method to return instances of <see cref="T:Xunit.Sdk.ITestCommand"/>, one per execution of a test method.
        /// </summary>
        /// <param name="method">The test method</param>
        /// <returns>The test commands which will execute the test runs for the given method.</returns>
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (method.MethodInfo != null && method.MethodInfo.GetParameters().Any())
            {
                var theoryTestCommands = base.EnumerateTestCommands(method);
                return GetTheoryCommands(method, theoryTestCommands);
            }
            else
            {
                return GetFactCommands(method);
            }
        }

        private static void RegisterSteps(IMethodInfo method)
        {
            if (method.IsStatic)
            {
                method.Invoke(null, null);
            }
            else
            {
                var type = method.MethodInfo.ReflectedType;
                var defaultConstructor = type.GetConstructor(Type.EmptyTypes);
                if (defaultConstructor == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The type '{0}' does not have a default constructor", type.Name));
                }

                var instance = defaultConstructor.Invoke(null);
                method.Invoke(instance, null);
            }
        }
    }
}
