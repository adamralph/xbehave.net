// <copyright file="ScenarioTestGroup.cs" company="xBehave.net contributors">
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

    public class ScenarioTestGroup : ITestGroup, IDisposable
    {
        private static readonly ITypeInfo objectTypeInfo = Reflector.Wrap(typeof(object));

        private readonly IXunitTestCase testCase;
        private readonly string displayName;
        private readonly Type testClass;
        private readonly MethodInfo testMethod;
        private readonly object[] testMethodArguments;
        private readonly string skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes;

        public ScenarioTestGroup(
            IXunitTestCase testCase,
            string baseDisplayName,
            Type testClass,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes)
        {
            Guard.AgainstNullArgument("testCase", testCase);
            Guard.AgainstNullArgumentProperty("testCase", "TestMethod", testCase.TestMethod);
            Guard.AgainstNullArgumentProperty("testCase", "TestMethod.Method", testCase.TestMethod.Method);
            Guard.AgainstNullArgument("testMethod", testMethod);
            Guard.AgainstNullArgument("testMethodArguments", testMethodArguments);

            var typeArguments = new ITypeInfo[0];
            var closedMethod = testMethod;
            if (closedMethod.IsGenericMethodDefinition)
            {
                typeArguments = ResolveTypeArguments(testCase.TestMethod.Method, testMethodArguments.ToArray()).ToArray();

                closedMethod =
                    closedMethod.MakeGenericMethod(typeArguments.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
            }

            var parameterTypes = closedMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var convertedArgumentValues = Reflector.ConvertArguments(testMethodArguments, parameterTypes);

            var parameters = testCase.TestMethod.Method.GetParameters().ToArray();
            var generatedArguments = new List<Argument>();
            for (var missingArgumentIndex = testMethodArguments.Length;
                missingArgumentIndex < parameters.Length;
                ++missingArgumentIndex)
            {
                var parameterType = parameters[missingArgumentIndex].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    ITypeInfo concreteType = null;
                    var typeParameters = testCase.TestMethod.Method.GetGenericArguments().ToArray();
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

            this.testCase = testCase;
            this.displayName = GetScenarioTestGroupDisplayName(testCase.TestMethod.Method, baseDisplayName, arguments, typeArguments);
            this.testClass = testClass;
            this.testMethod = closedMethod;
            this.testMethodArguments = arguments.Select(argument => argument.Value).ToArray();
            this.skipReason = skipReason;
            this.beforeAfterTestGroupAttributes = beforeAfterTestGroupAttributes;
        }

        ~ScenarioTestGroup()
        {
            this.Dispose(false);
        }

        public IXunitTestCase TestCase
        {
            get { return this.testCase; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        ITestCase ITestGroup.TestCase
        {
            get { return this.testCase; }
        }

        protected Type TestClass
        {
            get { return this.testClass; }
        }

        protected MethodInfo TestMethod
        {
            get { return this.testMethod; }
        }

        protected IReadOnlyList<object> TestMethodArguments
        {
            get { return this.testMethodArguments; }
        }

        protected string SkipReason
        {
            get { return this.skipReason; }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterTestGroupAttributes
        {
            get { return this.beforeAfterTestGroupAttributes; }
        }

        public virtual Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return new ScenarioTestGroupRunner(
                    this,
                    messageBus,
                    this.testClass,
                    constructorArguments,
                    this.testMethod,
                    this.testMethodArguments,
                    this.skipReason,
                    this.beforeAfterTestGroupAttributes,
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

        private static string GetScenarioTestGroupDisplayName(
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
