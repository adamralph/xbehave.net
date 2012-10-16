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
    using System.Reflection;
    using Xbehave.Sdk;
    using Xunit;
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
    public class ScenarioAttribute : FactAttribute
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

            var parameters = method.MethodInfo.GetParameters();
            if (!parameters.Any())
            {
                return new[] { new TheoryCommand(method, new object[0]) };
            }

            var commands = new List<ITestCommand>();
            try
            {
                foreach (var argumentList in GetArgumentLists(method.MethodInfo))
                {
                    var closedTypeMethod = method;
                    Type[] typeArguments = null;
                    if (method.MethodInfo != null && method.MethodInfo.IsGenericMethodDefinition)
                    {
                        typeArguments = ResolveTypeArguments(method, argumentList).ToArray();
                        closedTypeMethod = Reflector.Wrap(method.MethodInfo.MakeGenericMethod(typeArguments));
                    }

                    var generatedArguments = new List<object>();
                    for (var missingArgumentIndex = argumentList.Length; missingArgumentIndex < parameters.Length; ++missingArgumentIndex)
                    {
                        var parameterType = parameters[missingArgumentIndex].ParameterType;
                        if (parameterType.IsGenericParameter)
                        {
                            Type concreteType = null;
                            var genericTypes = method.MethodInfo.GetGenericArguments();
                            for (var genericTypeIndex = 0; genericTypeIndex < genericTypes.Length; ++genericTypeIndex)
                            {
                                if (genericTypes[genericTypeIndex] == parameterType)
                                {
                                    concreteType = typeArguments[genericTypeIndex];
                                    break;
                                }
                            }

                            if (concreteType == null)
                            {
                                var message = string.Format(
                                    CultureInfo.CurrentCulture, "The type of parameter \"{0}\" cannot be resolved.", parameters[missingArgumentIndex].Name);
                                throw new InvalidOperationException(message);
                            }

                            parameterType = concreteType;
                        }

                        if (parameterType.IsValueType)
                        {
                            generatedArguments.Add(Activator.CreateInstance(parameterType));
                        }
                        else
                        {
                            generatedArguments.Add(null);
                        }
                    }

                    commands.Add(new TheoryCommand(closedTypeMethod, argumentList.Concat(generatedArguments).ToArray(), typeArguments));
                }

                if (commands.Count == 0)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, "No data found for {0}.{1}", method.TypeName, method.Name);
                    commands.Add(new ExceptionCommand(method, new InvalidOperationException(message)));
                }
            }
            catch (Exception ex)
            {
                commands.Clear();
                var message = string.Format(
                    CultureInfo.CurrentCulture, "An exception was thrown while getting data for scenario {0}.{1}:\r\n{2}", method.TypeName, method.Name, ex);
                commands.Add(new ExceptionCommand(method, new InvalidOperationException(message)));
            }

            return commands;
        }

        private static IEnumerable<object[]> GetArgumentLists(MethodInfo method)
        {
            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            var dataAttributes = method.GetCustomAttributes(typeof(DataAttribute), false).Cast<DataAttribute>().ToArray();
            foreach (var dataAttribute in dataAttributes)
            {
                var argumentLists = dataAttribute.GetData(method, parameterTypes);
                if (argumentLists != null)
                {
                    foreach (var argumentList in argumentLists)
                    {
                        yield return argumentList;
                    }
                }
            }

            if (dataAttributes.Length == 0)
            {
                yield return parameterTypes.Select(type => type.IsValueType ? Activator.CreateInstance(type) : null).ToArray();
            }
        }

        private static IEnumerable<Type> ResolveTypeArguments(IMethodInfo method, object[] arguments)
        {
            var parameters = method.MethodInfo.GetParameters();
            return method.MethodInfo.GetGenericArguments().Select(genericType => ResolveGenericType(genericType, parameters, arguments));
        }

        private static Type ResolveGenericType(Type genericType, ParameterInfo[] parameters, object[] arguments)
        {
            var sawNullValue = false;
            Type type = null;
            for (var index = 0; index < Math.Min(parameters.Length, arguments.Length); ++index)
            {
                if (parameters[index].ParameterType == genericType)
                {
                    var argument = arguments[index];
                    if (argument == null)
                    {
                        sawNullValue = true;
                    }
                    else if (type == null)
                    {
                        type = argument.GetType();
                    }
                    else if (type != argument.GetType())
                    {
                        return typeof(object);
                    }
                }
            }

            if (type == null)
            {
                return typeof(object);
            }

            return sawNullValue && type.IsValueType ? typeof(object) : type;
        }
    }
}
