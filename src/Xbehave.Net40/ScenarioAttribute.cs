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
    using Guard = Xbehave.Sdk.Guard;

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
            Guard.AgainstNullArgument("method", method);

            IEnumerable<ITestCommand> backgroundCommands;
            IEnumerable<ITestCommand> scenarioCommands;

            // NOTE: any exception must be wrapped in a command, otherwise the test runner will retry this method infinitely
            try
            {
                backgroundCommands = this.EnumerateBackgroundCommands(method).ToArray();
                scenarioCommands = this.EnumerateScenarioCommands(method).ToArray();
            }
            catch (Exception ex)
            {
                return new[] { new ExceptionCommand(method, ex) };
            }

            // NOTE: this is not in the try catch since we are yielding internally
            // TODO: address this - see http://stackoverflow.com/a/346772/49241
            return scenarioCommands.SelectMany(scenarioCommand =>
            {
                var theoryCommand = scenarioCommand as TheoryCommand;
                var args = theoryCommand == null ? new object[0] : theoryCommand.Parameters;
                return CurrentScenario.ExtractCommands(method, args, backgroundCommands.Concat(new[] { scenarioCommand }));
            });
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

            return method.MethodInfo.GetParameters().Any() ? EnumerateTestCommands2(method) : new[] { new TheoryCommand(method, new object[0]) };
        }

        private static IEnumerable<object[]> GetData(MethodInfo method)
        {
            foreach (DataAttribute attr in method.GetCustomAttributes(typeof(DataAttribute), false))
            {
                var parameterInfos = method.GetParameters();
                var parameterTypes = new Type[parameterInfos.Length];
                for (int idx = 0; idx < parameterInfos.Length; idx++)
                {
                    parameterTypes[idx] = parameterInfos[idx].ParameterType;
                }

                var attrData = attr.GetData(method, parameterTypes);
                if (attrData != null)
                {
                    foreach (object[] dataItems in attrData)
                    {
                        yield return dataItems;
                    }
                }
            }
        }

        private static Type ResolveGenericType(Type genericType, object[] parameters, ParameterInfo[] parameterInfos)
        {
            bool sawNullValue = false;
            Type matchedType = null;

            for (int idx = 0; idx < parameterInfos.Length; ++idx)
            {
                if (parameterInfos[idx].ParameterType == genericType)
                {
                    object parameterValue = parameters[idx];

                    if (parameterValue == null)
                    {
                        sawNullValue = true;
                    }
                    else if (matchedType == null)
                    {
                        matchedType = parameterValue.GetType();
                    }
                    else if (matchedType != parameterValue.GetType())
                    {
                        return typeof(object);
                    }
                }
            }

            if (matchedType == null)
            {
                return typeof(object);
            }

            return sawNullValue && matchedType.IsValueType ? typeof(object) : matchedType;
        }

        private static Type[] ResolveGenericTypes(IMethodInfo method, object[] parameters)
        {
            var genericTypes = method.MethodInfo.GetGenericArguments();
            var resolvedTypes = new Type[genericTypes.Length];
            var parameterInfos = method.MethodInfo.GetParameters();
            for (int idx = 0; idx < genericTypes.Length; ++idx)
            {
                resolvedTypes[idx] = ResolveGenericType(genericTypes[idx], parameters, parameterInfos);
            }

            return resolvedTypes;
        }

        private static IEnumerable<ITestCommand> EnumerateTestCommands2(IMethodInfo method)
        {
            var results = new List<ITestCommand>();
            try
            {
                foreach (var exampleArgs in GetData(method.MethodInfo))
                {
                    var testMethod = method;
                    Type[] resolvedTypes = null;

                    if (method.MethodInfo != null && method.MethodInfo.IsGenericMethodDefinition)
                    {
                        resolvedTypes = ResolveGenericTypes(method, exampleArgs);
                        testMethod = Reflector.Wrap(method.MethodInfo.MakeGenericMethod(resolvedTypes));
                    }

                    var parameterTypes = method.MethodInfo.GetParameters().Select(info => info.ParameterType);
                    var defaultArgs = new List<object>();
                    foreach (var type in parameterTypes.Skip(exampleArgs.Length))
                    {
                        defaultArgs.Add(IsNullableType(type) ? null : Activator.CreateInstance(type));
                    }

                    results.Add(new TheoryCommand(testMethod, exampleArgs.Concat(defaultArgs).ToArray(), resolvedTypes));
                }

                if (results.Count == 0)
                {
                    var command = new ExceptionCommand(
                        method,
                        new InvalidOperationException(string.Format("No data found for {0}.{1}", method.TypeName, method.Name)));
                    results.Add(command);
                }
            }
            catch (Exception ex)
            {
                results.Clear();
                var message = string.Format(
                    "An exception was thrown while getting data for theory {0}.{1}:\r\n{2}", method.TypeName, method.Name, ex);
                results.Add(new ExceptionCommand(method, new InvalidOperationException(message)));
            }

            return results;
        }

        private static bool IsNullableType(Type type)
        {
            return !type.IsValueType || IsNullableValueType(type);
        }

        private static bool IsNullableValueType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
