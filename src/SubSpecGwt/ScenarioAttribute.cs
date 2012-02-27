// <copyright file="ScenarioAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            return method.MethodInfo.GetParameters().Any()
                ? this.GetTheoryCommands(method)
                : GetFactCommands(method);
        }

        private static IEnumerable<ITestCommand> GetFactCommands(IMethodInfo method)
        {
            return Core.SpecificationContext.SafelyEnumerateTestCommands(method, RegisterSpecificationPrimitives);
        }

        private static void RegisterSpecificationPrimitives(IMethodInfo method)
        {
            if (method.IsStatic)
            {
                method.Invoke(null, null);
            }
            else
            {
                var defaultConstructor = method.MethodInfo.ReflectedType.GetConstructor(Type.EmptyTypes);
                if (defaultConstructor == null)
                {
                    throw new InvalidOperationException("Specification class does not have a default constructor");
                }

                var instance = defaultConstructor.Invoke(null);
                method.Invoke(instance, null);
            }
        }

        private IEnumerable<ITestCommand> GetTheoryCommands(IMethodInfo method)
        {
            var theoryTestCommands = base.EnumerateTestCommands(method);
            var commands = new List<ITestCommand>();
            foreach (var item in theoryTestCommands)
            {
                if (item is TheoryCommand)
                {
                    var itemCommands = Core.SpecificationContext.SafelyEnumerateTestCommands(
                        method,
                        m => item.Execute(item.ShouldCreateInstance ? Activator.CreateInstance(method.MethodInfo.ReflectedType) : null));

                    commands.AddRange(itemCommands);
                }
                else
                {
                    commands.Clear();
                    commands.Add(item);
                    break;
                }
            }

            return commands;
        }
    }
}
