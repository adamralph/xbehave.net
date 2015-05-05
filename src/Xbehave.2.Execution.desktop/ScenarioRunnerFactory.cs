// <copyright file="ScenarioRunnerFactory.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Xbehave.Execution.Extensions;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunnerFactory
    {
        private static readonly ITypeInfo objectTypeInfo = Reflector.Wrap(typeof(object));

        private readonly IXunitTestCase scenarioOutline;
        private readonly MethodInfo scenarioMethod;
        private readonly string baseDisplayName;
        private readonly IMessageBus messageBus;
        private readonly Type scenarioClass;
        private readonly object[] constructorArguments;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes;
        private readonly string skipReason;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;

        public ScenarioRunnerFactory(
            IXunitTestCase scenarioOutline,
            string baseDisplayName,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            MethodInfo scenarioMethod,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("scenarioOutline", scenarioOutline);
            Guard.AgainstNullArgumentProperty("scenarioOutline", "TestMethod", scenarioOutline.TestMethod);
            Guard.AgainstNullArgumentProperty("scenarioOutline", "TestMethod.Method", scenarioOutline.TestMethod.Method);

            Guard.AgainstNullArgument("scenarioMethod", scenarioMethod);

            this.scenarioOutline = scenarioOutline;
            this.baseDisplayName = baseDisplayName;
            this.messageBus = messageBus;
            this.scenarioClass = scenarioClass;
            this.constructorArguments = constructorArguments;
            this.scenarioMethod = scenarioMethod;
            this.skipReason = skipReason;
            this.beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public ScenarioRunner Create(object[] scenarioMethodArguments)
        {
            var parameters = this.scenarioOutline.TestMethod.Method.GetParameters().ToArray();

            ITypeInfo[] typeArguments;
            MethodInfo closedScenarioMethod;
            if (this.scenarioMethod.IsGenericMethodDefinition)
            {
                typeArguments = this.scenarioOutline.TestMethod.Method.GetGenericArguments()
                    .Select(typeParameter =>
                        ResolveTypeArgument(typeParameter, parameters, scenarioMethodArguments.ToArray()))
                    .ToArray();

                closedScenarioMethod = this.scenarioMethod.MakeGenericMethod(
                    typeArguments.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
            }
            else
            {
                typeArguments = new ITypeInfo[0];
                closedScenarioMethod = this.scenarioMethod;
            }

            var parameterTypes = closedScenarioMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var convertedArgumentValues = Reflector.ConvertArguments(scenarioMethodArguments, parameterTypes);

            var generatedArguments = new List<Argument>();
            for (var missingArgumentIndex = scenarioMethodArguments.Length;
                missingArgumentIndex < parameters.Length;
                ++missingArgumentIndex)
            {
                var parameterType = parameters[missingArgumentIndex].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    ITypeInfo concreteType = null;
                    var typeParameters = this.scenarioOutline.TestMethod.Method.GetGenericArguments().ToArray();
                    for (var typeParameterIndex = 0; typeParameterIndex < typeParameters.Length; ++typeParameterIndex)
                    {
                        var typeParameter = typeParameters[typeParameterIndex];
                        if (typeParameter.Name == parameterType.Name)
                        {
                            concreteType = typeArguments[typeParameterIndex];
                            break;
                        }
                    }

                    if (concreteType == null)
                    {
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "The type of parameter \"{0}\" cannot be resolved.",
                            parameters[missingArgumentIndex].Name);

                        throw new InvalidOperationException(message);
                    }

                    parameterType = concreteType;
                }

                generatedArguments.Add(new Argument(((IReflectionTypeInfo)parameterType).Type));
            }

            var arguments = convertedArgumentValues
                .Select(value => new Argument(value))
                .Concat(generatedArguments)
                .ToArray();

            var scenarioDisplayName = GetScenarioDisplayName(
                this.scenarioOutline.Method, this.baseDisplayName, arguments, typeArguments);

            return new ScenarioRunner(
                new Scenario(this.scenarioOutline, scenarioDisplayName),
                this.messageBus,
                this.scenarioClass,
                this.constructorArguments,
                closedScenarioMethod,
                arguments.Select(argument => argument.Value).ToArray(),
                this.skipReason,
                this.beforeAfterScenarioAttributes,
                new ExceptionAggregator(this.aggregator),
                this.cancellationTokenSource);
        }

        private static ITypeInfo ResolveTypeArgument(
            ITypeInfo typeParameter, IList<IParameterInfo> parameters, IList<object> argumentValues)
        {
            var sawNullValue = false;
            ITypeInfo type = null;
            for (var index = 0; index < Math.Min(parameters.Count, argumentValues.Count); ++index)
            {
                var parameterType = parameters[index].ParameterType;
                if (parameterType.IsGenericParameter && parameterType.Name == typeParameter.Name)
                {
                    var argumentValue = argumentValues[index];
                    if (argumentValue == null)
                    {
                        sawNullValue = true;
                    }
                    else if (type == null)
                    {
                        type = Reflector.Wrap(argumentValue.GetType());
                    }
                    else if (type.Name != argumentValue.GetType().FullName)
                    {
                        return objectTypeInfo;
                    }
                }
            }

            if (type == null)
            {
                return objectTypeInfo;
            }

            return sawNullValue && type.IsValueType ? objectTypeInfo : type;
        }

        private static string GetScenarioDisplayName(
            IMethodInfo method, string baseDisplayName, Argument[] arguments, ITypeInfo[] typeArguments)
        {
            if (typeArguments.Length > 0)
            {
                baseDisplayName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}<{1}>",
                    baseDisplayName,
                    string.Join(", ", typeArguments.Select(typeArgument => typeArgument.ToSimpleString())));
            }

            var parameterTokens = new List<string>();
            var parameters = method.GetParameters().ToArray();
            int parameterIndex;
            for (parameterIndex = 0; parameterIndex < arguments.Length; parameterIndex++)
            {
                if (arguments[parameterIndex].IsGeneratedDefault)
                {
                    continue;
                }

                parameterTokens.Add(string.Concat(
                    parameterIndex >= parameters.Length ? "???" : parameters[parameterIndex].Name,
                    ": ",
                    arguments[parameterIndex].ToString()));
            }

            for (; parameterIndex < parameters.Length; parameterIndex++)
            {
                parameterTokens.Add(parameters[parameterIndex].Name + ": ???");
            }

            return string.Format(
                CultureInfo.InvariantCulture, "{0}({1})", baseDisplayName, string.Join(", ", parameterTokens));
        }
    }
}
