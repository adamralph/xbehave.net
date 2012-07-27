// <copyright file="ScenarioAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Xbehave.Sdk;
    using Xunit.Extensions;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    /// <summary>
    /// Applied to a method to indicate the definition of a scenario.
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
        /// Enumerates the test commands representing the background and scenario steps for each isolated context.
        /// </summary>
        /// <param name="method">The scenario method</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the background and scenario steps for each isolated context.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to avoid infinite loop in test runner.")]
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            IEnumerable<ITestCommand> backgroundCommands;
            IEnumerable<ITestCommand> scenarioCommands;
            object feature;

            // NOTE: any exception must be wrapped in a command, otherwise the test runner will retry this method infinitely
            try
            {
                backgroundCommands = this.EnumerateBackgroundCommands(method).ToArray();
                scenarioCommands = this.EnumerateScenarioCommands(method).ToArray();
                feature = method.IsStatic ? null : method.CreateInstance();
            }
            catch (Exception ex)
            {
                return new[] { new ExceptionCommand(method, ex) };
            }

            // NOTE: this is not in the try catch since we are yielding internally
            // TODO: address this - see http://stackoverflow.com/a/346772/49241
            return scenarioCommands.SelectMany(scenarioCommand =>
                 CurrentScenario.ExtractCommands(
                    method, ResolveTypeArguments(method, scenarioCommand.GetParameters()), scenarioCommand.GetParameters(), backgroundCommands, scenarioCommand, feature));
        }

        /// <summary>
        /// Enumerates the commands representing the backgrounds associated with the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The scenario method</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the backgrounds associated with the <paramref name="method"/>.</returns>
        protected virtual IEnumerable<ITestCommand> EnumerateBackgroundCommands(IMethodInfo method)
        {
            Guard.AgainstNullArgument("method", method);
            Guard.AgainstNullArgumentProperty("method", "Class", method.Class);

            return method.Class.GetMethods().SelectMany(
                candidateMethod => candidateMethod.GetCustomAttributes(typeof(BackgroundAttribute))
                    .Select(attribute => attribute.GetInstance<BackgroundAttribute>())
                    .SelectMany(backgroundAttribute => backgroundAttribute.CreateBackgroundCommands(candidateMethod))).ToArray();
        }

        /// <summary>
        /// Enumerates the commands representing the scenarios defined by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The scenario method</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the scenarios defined by the <paramref name="method"/>.</returns>
        /// <remarks>This method may be overridden.</remarks>
        protected virtual IEnumerable<ITestCommand> EnumerateScenarioCommands(IMethodInfo method)
        {
            Guard.AgainstNullArgument("method", method);
            Guard.AgainstNullArgumentProperty("method", "MethodInfo", method.MethodInfo);

            return method.MethodInfo.GetParameters().Any()
                ? base.EnumerateTestCommands(method)
                : new[] { new TheoryCommand(method, new object[0]) };
        }

        private static IEnumerable<Type> ResolveTypeArguments(IMethodInfo genericMethodDefinition, object[] arguments)
        {
            var genericParameters = genericMethodDefinition.MethodInfo.GetParameters();
            return genericMethodDefinition.MethodInfo.GetGenericArguments().Select(typeParameter => ResolveTypeArgument(typeParameter, genericParameters, arguments));
        }

        private static Type ResolveTypeArgument(Type typeParameter, ParameterInfo[] genericParameters, object[] arguments)
        {
            Type typeArgument = null;
            for (var index = 0; index < genericParameters.Length; ++index)
            {
                if (genericParameters[index].ParameterType != typeParameter)
                {
                    continue;
                }

                var argument = arguments[index];
                if (argument == null)
                {
                    continue;
                }

                if (typeArgument == null)
                {
                    typeArgument = argument.GetType();
                }
                else if (typeArgument != argument.GetType())
                {
                    return typeof(object);
                }
            }

            return typeArgument ?? typeof(object);
        }
    }
}
