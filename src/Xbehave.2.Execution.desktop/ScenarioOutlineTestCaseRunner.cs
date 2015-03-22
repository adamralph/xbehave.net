// <copyright file="ScenarioOutlineTestCaseRunner.cs" company="xBehave.net contributors">
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

    public class ScenarioOutlineTestCaseRunner : XunitTestCaseRunner
    {
        private static readonly object[] noArguments = new object[0];
        private static readonly ITypeInfo objectTypeInfo = Reflector.Wrap(typeof(object));

        private readonly ExceptionAggregator cleanupAggregator = new ExceptionAggregator();
        private readonly List<ScenarioTestGroup> scenarioTestGroups = new List<ScenarioTestGroup>();
        private Exception dataDiscoveryException;

        public ScenarioOutlineTestCaseRunner(
            IMessageSink diagnosticMessageSink,
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                noArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
            this.DiagnosticMessageSink = diagnosticMessageSink;
        }

        protected IMessageSink DiagnosticMessageSink { get; set; }

        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            var scenarioNumber = 1;
            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute)).ToList();
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var discoverer =
                        ExtensibilityPointFactory.GetDataDiscoverer(this.DiagnosticMessageSink, discovererAttribute);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, TestCase.TestMethod.Method))
                    {
                        var scenarioTestGroup = CreateScenarioTestGroup(
                            this.TestCase,
                            this.DisplayName,
                            scenarioNumber++,
                            this.TestClass,
                            this.TestMethod,
                            dataRow,
                            this.SkipReason,
                            this.BeforeAfterAttributes);

                        this.scenarioTestGroups.Add(scenarioTestGroup);
                    }
                }

                if (!this.scenarioTestGroups.Any())
                {
                    var scenarioTestGroup = CreateScenarioTestGroup(
                        this.TestCase,
                        this.DisplayName,
                        1,
                        this.TestClass,
                        this.TestMethod,
                        noArguments,
                        this.SkipReason,
                        this.BeforeAfterAttributes);

                    this.scenarioTestGroups.Add(scenarioTestGroup);
                }
            }
            catch (Exception ex)
            {
                this.dataDiscoveryException = ex;
            }
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            if (this.dataDiscoveryException != null)
            {
                this.MessageBus.Queue(
                    new XunitTest(TestCase, DisplayName),
                    test => new TestFailed(test, 0, null, this.dataDiscoveryException.Unwrap()),
                    this.CancellationTokenSource);

                return new RunSummary { Total = 1, Failed = 1 };
            }

            var summary = new RunSummary();
            foreach (var scenarioTestGroup in this.scenarioTestGroups)
            {
                summary.Aggregate(await scenarioTestGroup.RunAsync(
                        this.DiagnosticMessageSink,
                        this.MessageBus,
                        this.ConstructorArguments,
                        new ExceptionAggregator(this.Aggregator),
                        this.CancellationTokenSource));
            }

            // Run the cleanup here so we can include cleanup time in the run summary,
            // but save any exceptions so we can surface them during the cleanup phase,
            // so they get properly reported as test case cleanup failures.
            var timer = new ExecutionTimer();
            foreach (var scenarioTestGroup in this.scenarioTestGroups)
            {
                timer.Aggregate(() => this.cleanupAggregator.Run(() => scenarioTestGroup.Dispose()));
            }

            summary.Time += timer.Total;
            return summary;
        }

        protected override Task BeforeTestCaseFinishedAsync()
        {
            Aggregator.Aggregate(this.cleanupAggregator);

            return base.BeforeTestCaseFinishedAsync();
        }

        private static ScenarioTestGroup CreateScenarioTestGroup(
            IXunitTestCase testCase,
            string baseDisplayName,
            int scenarioNumber,
            Type testClass,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes)
        {
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

            return new ScenarioTestGroup(
                testCase,
                GetScenarioTestGroupDisplayName(testCase.TestMethod.Method, baseDisplayName, arguments, typeArguments),
                scenarioNumber,
                testClass,
                closedMethod,
                arguments.Select(argument => argument.Value).ToArray(),
                skipReason,
                beforeAfterTestGroupAttributes);
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
