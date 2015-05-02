// <copyright file="Scenario.cs" company="xBehave.net contributors">
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
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    // TODO: this cannot inherit from ITest and have all these fields. ITest needs to be xunit serializable
    public class Scenario : ITest, IDisposable
    {
        private static readonly ITypeInfo objectTypeInfo = Reflector.Wrap(typeof(object));

        private readonly IXunitTestCase scenarioOutline;
        private readonly string displayName;
        private readonly Type scenarioClass;
        private readonly MethodInfo scenarioMethod;
        private readonly object[] testMethodArguments;
        private readonly string skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes;

        public Scenario(
            IXunitTestCase scenarioOutline,
            string baseDisplayName,
            Type scenarioClass,
            MethodInfo scenarioMethod,
            object[] scenarioMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes)
        {
            Guard.AgainstNullArgument("scenarioOutline", scenarioOutline);
            Guard.AgainstNullArgumentProperty("scenarioOutline", "TestMethod", scenarioOutline.TestMethod);
            Guard.AgainstNullArgumentProperty("scenarioOutline", "TestMethod.Method", scenarioOutline.TestMethod.Method);
            Guard.AgainstNullArgument("scenarioMethod", scenarioMethod);
            Guard.AgainstNullArgument("scenarioMethodArguments", scenarioMethodArguments);

            var typeArguments = new ITypeInfo[0];
            var closedMethod = scenarioMethod;
            if (closedMethod.IsGenericMethodDefinition)
            {
                typeArguments = ResolveTypeArguments(scenarioOutline.TestMethod.Method, scenarioMethodArguments.ToArray()).ToArray();

                closedMethod =
                    closedMethod.MakeGenericMethod(typeArguments.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
            }

            var parameterTypes = closedMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var convertedArgumentValues = Reflector.ConvertArguments(scenarioMethodArguments, parameterTypes);

            var parameters = scenarioOutline.TestMethod.Method.GetParameters().ToArray();
            var generatedArguments = new List<Argument>();
            for (var missingArgumentIndex = scenarioMethodArguments.Length;
                missingArgumentIndex < parameters.Length;
                ++missingArgumentIndex)
            {
                var parameterType = parameters[missingArgumentIndex].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    ITypeInfo concreteType = null;
                    var typeParameters = scenarioOutline.TestMethod.Method.GetGenericArguments().ToArray();
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

            this.scenarioOutline = scenarioOutline;
            this.displayName = GetScenarioDisplayName(scenarioOutline.TestMethod.Method, baseDisplayName, arguments, typeArguments);
            this.scenarioClass = scenarioClass;
            this.scenarioMethod = closedMethod;
            this.testMethodArguments = arguments.Select(argument => argument.Value).ToArray();
            this.skipReason = skipReason;
            this.beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
        }

        ~Scenario()
        {
            this.Dispose(false);
        }

        public IXunitTestCase ScenarioOutline
        {
            get { return this.scenarioOutline; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        ITestCase ITest.TestCase
        {
            get { return this.scenarioOutline; }
        }

        protected Type TestClass
        {
            get { return this.scenarioClass; }
        }

        protected MethodInfo TestMethod
        {
            get { return this.scenarioMethod; }
        }

        protected IReadOnlyList<object> TestMethodArguments
        {
            get { return this.testMethodArguments; }
        }

        protected string SkipReason
        {
            get { return this.skipReason; }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterScenarioAttributes
        {
            get { return this.beforeAfterScenarioAttributes; }
        }

        public virtual Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return new ScenarioRunner(
                    this,
                    messageBus,
                    this.scenarioClass,
                    constructorArguments,
                    this.scenarioMethod,
                    this.testMethodArguments,
                    this.skipReason,
                    this.beforeAfterScenarioAttributes,
                    aggregator,
                    cancellationTokenSource)
                .RunAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.testMethodArguments != null)
            {
                foreach (var disposable in this.testMethodArguments.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
            }
        }

        private static IEnumerable<ITypeInfo> ResolveTypeArguments(IMethodInfo method, IList<object> argumentValues)
        {
            var parameters = method.GetParameters().ToArray();
            return method.GetGenericArguments()
                .Select(typeParameter => ResolveTypeArgument(typeParameter, parameters, argumentValues));
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
