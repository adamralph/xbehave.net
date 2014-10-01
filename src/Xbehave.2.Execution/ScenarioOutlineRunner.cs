// <copyright file="ScenarioOutlineRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
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
            var runners = new List<ScenarioRunner>();
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
                        runners.Add(this.CreateRunner(disposables, dataRow));
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

            if (!runners.Any())
            {
                runners.Add(this.CreateRunner(disposables, NoArguments));
            }

            var summary = new RunSummary();
            foreach (var runner in runners)
            {
                summary.Aggregate(await runner.RunAsync());
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

        private ScenarioRunner CreateRunner(List<IDisposable> disposables, object[] testMethodArguments)
        {
            disposables.AddRange(testMethodArguments.OfType<IDisposable>());

            ITypeInfo[] resolvedTypes = null;
            var testMethod = TestMethod;
            if (testMethod.IsGenericMethodDefinition)
            {
                resolvedTypes = Xunit.Sdk.TypeUtility.ResolveGenericTypes(TestCase.TestMethod.Method, testMethodArguments);
                testMethod = testMethod.MakeGenericMethod(
                    resolvedTypes.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
            }

            var parameterTypes = testMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var convertedTestMethodArguments = Reflector.ConvertArguments(testMethodArguments, parameterTypes);
            var displayName = Xunit.Sdk.TypeUtility.GetDisplayNameWithArguments(
                TestCase.TestMethod.Method, this.DisplayName, convertedTestMethodArguments, resolvedTypes);

            return new ScenarioRunner(
                TestCase,
                displayName,
                SkipReason,
                ConstructorArguments,
                convertedTestMethodArguments,
                MessageBus,
                Aggregator,
                CancellationTokenSource);
        }
    }
}
