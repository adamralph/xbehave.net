// <copyright file="ScenarioRunnerFactory.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using Xbehave.Execution.Extensions;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunnerFactory
    {
        private static readonly ITypeInfo objectType = Reflector.Wrap(typeof(object));

        private readonly IXunitTestCase scenarioOutline;
        private readonly IMethodInfo scenarioOutlineMethod;
        private readonly string scenarioOutlineDisplayName;
        private readonly IMessageBus messageBus;
        private readonly Type scenarioClass;
        private readonly object[] constructorArguments;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes;
        private readonly string skipReason;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;

        public ScenarioRunnerFactory(
            IXunitTestCase scenarioOutline,
            string scenarioOutlineDisplayName,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("scenarioOutline", scenarioOutline);
            Guard.AgainstNullArgumentProperty("scenarioOutline", "TestMethod", scenarioOutline.TestMethod);
            Guard.AgainstNullArgumentProperty("scenarioOutline", "TestMethod.Method", scenarioOutline.TestMethod.Method);

            this.scenarioOutline = scenarioOutline;
            this.scenarioOutlineMethod = scenarioOutline.TestMethod.Method;
            this.scenarioOutlineDisplayName = scenarioOutlineDisplayName;
            this.messageBus = messageBus;
            this.scenarioClass = scenarioClass;
            this.constructorArguments = constructorArguments;
            this.skipReason = skipReason;
            this.beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public ScenarioRunner Create(object[] scenarioMethodArguments)
        {
            var parameters = this.scenarioOutlineMethod.GetParameters().ToArray();
            var typeParameters = this.scenarioOutlineMethod.GetGenericArguments().ToArray();

            ITypeInfo[] typeArguments;
            MethodInfo scenarioMethod;
            if (this.scenarioOutlineMethod.IsGenericMethodDefinition)
            {
                typeArguments = typeParameters
                    .Select(typeParameter => InferTypeArgument(typeParameter.Name, parameters, scenarioMethodArguments))
                    .ToArray();

                scenarioMethod = this.scenarioOutlineMethod.MakeGenericMethod(typeArguments.ToArray()).ToRuntimeMethod();
            }
            else
            {
                typeArguments = new ITypeInfo[0];
                scenarioMethod = this.scenarioOutlineMethod.ToRuntimeMethod();
            }

            var passedArguments = Reflector.ConvertArguments(
                scenarioMethodArguments, scenarioMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            var generatedArguments = GetGeneratedArguments(
                typeParameters, typeArguments, parameters, passedArguments.Length);

            var arguments = passedArguments
                .Select(value => new Argument(value))
                .Concat(generatedArguments)
                .ToArray();

            var scenarioDisplayName = GetScenarioDisplayName(
                this.scenarioOutlineDisplayName, typeArguments, parameters, arguments);

            var scenario = new Scenario(this.scenarioOutline, scenarioDisplayName);

            return new ScenarioRunner(
                scenario,
                this.messageBus,
                this.scenarioClass,
                this.constructorArguments,
                scenarioMethod,
                arguments.Select(argument => argument.Value).ToArray(),
                this.skipReason,
                this.beforeAfterScenarioAttributes,
                new ExceptionAggregator(this.aggregator),
                this.cancellationTokenSource);
        }

        private static ITypeInfo InferTypeArgument(
            string typeParameterName, IReadOnlyList<IParameterInfo> parameters, IReadOnlyList<object> passedArguments)
        {
            var sawNullValue = false;
            ITypeInfo typeArgument = null;
            for (var index = 0; index < Math.Min(parameters.Count, passedArguments.Count); ++index)
            {
                var parameterType = parameters[index].ParameterType;
                if (parameterType.IsGenericParameter && parameterType.Name == typeParameterName)
                {
                    var passedArgument = passedArguments[index];
                    if (passedArgument == null)
                    {
                        sawNullValue = true;
                    }
                    else if (typeArgument == null)
                    {
                        typeArgument = Reflector.Wrap(passedArgument.GetType());
                    }
                    else if (typeArgument.Name != passedArgument.GetType().FullName)
                    {
                        return objectType;
                    }
                }
            }

            return typeArgument == null || (sawNullValue && typeArgument.IsValueType) ? objectType : typeArgument;
        }

        private static IEnumerable<Argument> GetGeneratedArguments(
            IReadOnlyList<ITypeInfo> typeParameters,
            IReadOnlyList<ITypeInfo> typeArguments,
            IReadOnlyList<IParameterInfo> parameters,
            int passedArgumentsCount)
        {
            for (var missingArgumentIndex = passedArgumentsCount;
                missingArgumentIndex < parameters.Count;
                ++missingArgumentIndex)
            {
                var parameterType = parameters[missingArgumentIndex].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    ITypeInfo concreteType = null;
                    for (var typeParameterIndex = 0; typeParameterIndex < typeParameters.Count; ++typeParameterIndex)
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

                yield return new Argument(((IReflectionTypeInfo)parameterType).Type);
            }
        }

        private static string GetScenarioDisplayName(
            string scenarioOutlineDisplayName,
            IReadOnlyList<ITypeInfo> typeArguments,
            IReadOnlyList<IParameterInfo> parameters,
            IReadOnlyList<Argument> arguments)
        {
            var typeArgumentsString = typeArguments.Any()
                ? string.Format(
                    CultureInfo.InvariantCulture,
                    "<{0}>",
                    string.Join(", ", typeArguments.Select(typeArgument => typeArgument.ToSimpleString())))
                : string.Empty;

            var parameterAndArgumentTokens = new List<string>();
            int parameterIndex;
            for (parameterIndex = 0; parameterIndex < arguments.Count; parameterIndex++)
            {
                if (arguments[parameterIndex].IsGeneratedDefault)
                {
                    continue;
                }

                parameterAndArgumentTokens.Add(string.Concat(
                    parameterIndex >= parameters.Count ? "???" : parameters[parameterIndex].Name,
                    ": ",
                    arguments[parameterIndex].ToString()));
            }

            for (; parameterIndex < parameters.Count; parameterIndex++)
            {
                parameterAndArgumentTokens.Add(parameters[parameterIndex].Name + ": ???");
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}({2})",
                scenarioOutlineDisplayName,
                typeArgumentsString,
                string.Join(", ", parameterAndArgumentTokens));
        }

        private class Argument
        {
            private static readonly MethodInfo genericFactoryMethod = CreateGenericFactoryMethod();

            private readonly object value;
            private readonly bool isGeneratedDefault;

            public Argument(Type type)
            {
                Guard.AgainstNullArgument("type", type);

                this.value = genericFactoryMethod.MakeGenericMethod(type).Invoke(null, null);
                this.isGeneratedDefault = true;
            }

            public Argument(object value)
            {
                this.value = value;
            }

            public object Value
            {
                get { return this.value; }
            }

            public bool IsGeneratedDefault
            {
                get { return this.isGeneratedDefault; }
            }

            public override string ToString()
            {
                return ArgumentFormatter.Format(this.value);
            }

            private static MethodInfo CreateGenericFactoryMethod()
            {
                Expression<Func<object>> template = () => CreateDefault<object>();
                return ((MethodCallExpression)template.Body).Method.GetGenericMethodDefinition();
            }

            private static T CreateDefault<T>()
            {
                return default(T);
            }
        }
    }
}
