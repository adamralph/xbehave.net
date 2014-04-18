// <copyright file="ScenarioAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
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
        /// Gets or sets a timeout which, if exceeded, indicates that the scenario has failed.
        /// </summary>
        /// <remarks>This property is deprecated in favor of timeouts on specific steps.</remarks>
        [Obsolete("Timeouts on scenarios have intermittent problems with xUnit.net test runners. Use the Timeout() extension method for steps instead.")]
        public new int Timeout { get; set; }

        /// <summary>
        /// Enumerates the test commands representing the background and scenario steps for each isolated context.
        /// </summary>
        /// <param name="method">The scenario method.</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the background and scenario steps for each isolated context.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to avoid infinite loop in test runner.")]
        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            if (method == null)
            {
                return new[] { new ExceptionCommand(new MethodCall(null), new ArgumentNullException("method")) };
            }

            IEnumerable<ITestCommand> backgroundCommands;
            IEnumerable<ICommand> scenarioCommands;

            // NOTE: any exception must be wrapped in a command, otherwise the test runner will retry this method infinitely
            try
            {
                backgroundCommands = this.EnumerateBackgroundCommands(method).ToArray();
                scenarioCommands = this.EnumerateScenarioCommands(method).ToArray();
            }
            catch (Exception ex)
            {
                return new[] { new ExceptionCommand(new MethodCall(method), ex) };
            }

            var continueOnFailureStepType = GetContinueOnFailureStepType(method);
            var omitArgumentsAttribute = GetCustomAttribute<OmitArgumentsFromScenarioNamesAttribute>(method);
            var omitArguments = omitArgumentsAttribute == null ? false : omitArgumentsAttribute.Enabled;

            // NOTE: this is not in the try catch since we are yielding internally
            // TODO: address this - see http://stackoverflow.com/a/346772/49241
            return scenarioCommands.SelectMany(c => CurrentScenario.ExtractCommands(c.MethodCall, backgroundCommands.Concat(new[] { c }), continueOnFailureStepType, omitArguments));
        }

        /// <summary>
        /// Enumerates the commands representing the backgrounds associated with the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The scenario method.</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the backgrounds associated with the <paramref name="method"/>.</returns>
        protected virtual IEnumerable<ITestCommand> EnumerateBackgroundCommands(IMethodInfo method)
        {
            Guard.AgainstNullArgument("method", method);
            Guard.AgainstNullArgumentProperty("method", "Class", method.Class);

            return method.Class
                         .GetMethods()
                         .SelectMany(candidateMethod =>
                             candidateMethod
                                .GetCustomAttributes(typeof(BackgroundAttribute))
                                .Select(attribute => attribute.GetInstance<BackgroundAttribute>())
                                .SelectMany(backgroundAttribute => backgroundAttribute.CreateBackgroundCommands(candidateMethod)))
                         .ToArray();
        }

        /// <summary>
        /// Enumerates the commands representing the scenarios defined by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The scenario method.</param>
        /// <returns>An instance of <see cref="IEnumerable{ITestCommand}"/> representing the scenarios defined by the <paramref name="method"/>.</returns>
        /// <remarks>This method may be overridden.</remarks>
        protected virtual IEnumerable<ICommand> EnumerateScenarioCommands(IMethodInfo method)
        {
            Guard.AgainstNullArgument("method", method);
            Guard.AgainstNullArgumentProperty("method", "MethodInfo", method.MethodInfo);

            var parameters = method.MethodInfo.GetParameters();
            if (!parameters.Any())
            {
                return new[] { new Command(new MethodCall(method)) };
            }

            var commands = new List<ICommand>();
            var ordinal = 0;
            foreach (var arguments in GetArgumentCollections(method.MethodInfo))
            {
                var closedTypeMethod = method;
                var typeArguments = new Type[0];
                if (method.MethodInfo != null && method.MethodInfo.IsGenericMethodDefinition)
                {
                    typeArguments = ResolveTypeArguments(method, arguments).ToArray();
                    closedTypeMethod = Reflector.Wrap(method.MethodInfo.MakeGenericMethod(typeArguments));
                }

                var generatedArguments = new List<Argument>();
                for (var missingArgumentIndex = arguments.Length; missingArgumentIndex < parameters.Length; ++missingArgumentIndex)
                {
                    var parameterType = parameters[missingArgumentIndex].ParameterType;
                    if (parameterType.IsGenericParameter)
                    {
                        Type concreteType = null;
                        if (method.MethodInfo != null)
                        {
                            var typeParameters = method.MethodInfo.GetGenericArguments();
                            for (var typeParameterIndex = 0; typeParameterIndex < typeParameters.Length; ++typeParameterIndex)
                            {
                                if (typeParameters[typeParameterIndex] == parameterType)
                                {
                                    concreteType = typeArguments[typeParameterIndex];
                                    break;
                                }
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

                    generatedArguments.Add(new Argument(parameterType));
                }

                var methodCall = new MethodCall(closedTypeMethod, arguments.Concat(generatedArguments).ToArray(), typeArguments, ++ordinal);
                commands.Add(new Command(methodCall));
            }

            return commands;
        }

        private static IEnumerable<Argument[]> GetArgumentCollections(MethodInfo method)
        {
            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            var dataAttributes = method.GetCustomAttributes(typeof(DataAttribute), false).Cast<DataAttribute>().ToArray();
            foreach (var dataAttribute in dataAttributes)
            {
                var datasets = dataAttribute.GetData(method, parameterTypes);
                if (datasets != null)
                {
                    foreach (var dataset in datasets)
                    {
                        yield return dataset.Select(datum => new Argument(datum)).ToArray();
                    }
                }
            }

            if (dataAttributes.Length == 0)
            {
                yield return parameterTypes.Select(type => new Argument(type)).ToArray();
            }
        }

        private static IEnumerable<Type> ResolveTypeArguments(IMethodInfo method, Argument[] arguments)
        {
            var parameters = method.MethodInfo.GetParameters();
            return method.MethodInfo.GetGenericArguments().Select(typeParameter => ResolveTypeArgument(typeParameter, parameters, arguments));
        }

        private static Type ResolveTypeArgument(Type typeParameter, ParameterInfo[] parameters, Argument[] arguments)
        {
            var sawNullValue = false;
            Type type = null;
            for (var index = 0; index < Math.Min(parameters.Length, arguments.Length); ++index)
            {
                if (parameters[index].ParameterType == typeParameter)
                {
                    var argument = arguments[index].Value;
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

        private static object GetContinueOnFailureStepType(IMethodInfo method)
        {
            var continueOnFailureAttribute = GetCustomAttribute<ContinueOnFailureAfterAttribute>(method);
            var continueOnFailureStepType = (object)false;

            if (continueOnFailureAttribute != null)
            {
                continueOnFailureStepType = continueOnFailureAttribute.StepType;
            }

            if (continueOnFailureStepType is StepType)
            {
                if ((StepType)continueOnFailureStepType == StepType.None)
                {
                    continueOnFailureStepType = false;
                }
                else if ((StepType)continueOnFailureStepType == StepType.Any)
                {
                    continueOnFailureStepType = true;
                }
            }

            return continueOnFailureStepType;
        }

        private static T GetCustomAttribute<T>(IMethodInfo methodInfo) where T : Attribute
        {
            return
                methodInfo.MethodInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T ??
                methodInfo.Class.Type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T ??
                methodInfo.Class.Type.Assembly.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }
    }
}
