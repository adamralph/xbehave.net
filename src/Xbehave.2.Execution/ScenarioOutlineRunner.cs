// <copyright file="ScenarioOutlineRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Shims;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioOutlineRunner : XunitTestCaseRunner
    {
        private static readonly object[] NoArguments = new object[0];

        public ScenarioOutlineRunner(
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
                NoArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var scenarioRunners = new List<ScenarioRunner>();
            var disposables = new List<IDisposable>();

            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute)).ToList();
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var discovererArguments = discovererAttribute.GetConstructorArguments().Cast<string>().ToList();
                    var discovererType = Reflector.GetType(discovererArguments[1], discovererArguments[0]);
                    var discoverer = ExtensibilityPointFactory.GetDataDiscoverer(discovererType);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, TestCase.TestMethod.Method))
                    {
                        scenarioRunners.Add(this.CreateRunner(disposables, dataRow));
                    }
                }
            }
            catch (Exception ex)
            {
                if (!MessageBus.QueueMessage(new TestStarting(TestCase, DisplayName)))
                {
                    CancellationTokenSource.Cancel();
                }
                else
                {
                    if (!MessageBus.QueueMessage(new TestFailed(TestCase, DisplayName, 0, null, ex.Unwrap())))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }

                if (!MessageBus.QueueMessage(new TestFinished(TestCase, DisplayName, 0, null)))
                {
                    CancellationTokenSource.Cancel();
                }

                return new RunSummary { Total = 1, Failed = 1 };
            }

            if (!scenarioRunners.Any())
            {
                scenarioRunners.Add(this.CreateRunner(disposables, new object[0]));
            }

            var summary = new RunSummary();
            foreach (var scenarioRunner in scenarioRunners)
            {
                summary.Aggregate(await scenarioRunner.RunAsync());
            }

            var timer = new ExecutionTimer();
            var aggregator = new ExceptionAggregator();

            foreach (var disposable in disposables)
            {
                timer.Aggregate(() => aggregator.Run(() => disposable.Dispose()));
            }

            summary.Time += timer.Total;
            return summary;
        }

        private ScenarioRunner CreateRunner(List<IDisposable> disposables, object[] argumentValues)
        {
            disposables.AddRange(argumentValues.OfType<IDisposable>());

            var typeArguments = new ITypeInfo[0];
            var closedMethod = TestMethod;
            if (closedMethod.IsGenericMethodDefinition)
            {
                typeArguments = TestCase.TestMethod.Method.ResolveGenericTypes(argumentValues.ToArray());

                closedMethod =
                    closedMethod.MakeGenericMethod(typeArguments.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
            }

            var parameterTypes = closedMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var convertedArgumentValues = Reflector.ConvertArguments(argumentValues, parameterTypes);

            var parameters = TestCase.TestMethod.Method.GetParameters().ToArray();
            var generatedArguments = new List<Argument>();
            for (var missingArgumentIndex = argumentValues.Length; missingArgumentIndex < parameters.Length; ++missingArgumentIndex)
            {
                var parameterType = parameters[missingArgumentIndex].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    ITypeInfo concreteType = null;
                    var typeParameters = TestCase.TestMethod.Method.GetGenericArguments().ToArray();
                    for (var typeParameterIndex = 0; typeParameterIndex < typeParameters.Length; ++typeParameterIndex)
                    {
                        if (typeParameters[typeParameterIndex] == parameterType)
                        {
                            concreteType = typeArguments[typeParameterIndex];
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

                generatedArguments.Add(new Argument(((IReflectionTypeInfo)parameterType).Type));
            }

            var arguments = convertedArgumentValues
                .Select(value => new Argument(value))
                .Concat(generatedArguments)
                .ToArray();

            var displayName = TypeUtility.GetDisplayNameWithArguments(
                TestCase.TestMethod.Method, this.DisplayName, arguments.Select(argument => argument.Value).ToArray(), typeArguments);

            return new ScenarioRunner(
                TestCase,
                displayName,
                SkipReason,
                ConstructorArguments,
                arguments.Select(argument => argument.Value).ToArray(),
                MessageBus,
                Aggregator,
                CancellationTokenSource);
        }
    }
}
