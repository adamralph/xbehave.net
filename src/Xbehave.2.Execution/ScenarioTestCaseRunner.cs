// <copyright file="ScenarioTestCaseRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioTestCaseRunner : TestCaseRunner<ScenarioTestCase>
    {
        private readonly object[] constructorArguments;
        private readonly string displayName;

        public ScenarioTestCaseRunner(
            ScenarioTestCase testCase,
            string displayName,
            object[] constructorArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
            this.displayName = displayName;
            this.constructorArguments = constructorArguments;
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var timer = new ExecutionTimer();
            var summary = new RunSummary();
            try
            {
                await timer.AggregateAsync(async () =>
                {
                    var type = Reflector.GetType(
                        this.TestCase.TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name,
                        this.TestCase.TestMethod.TestClass.Class.Name);

                    var method = type.GetMethod(
                        this.TestCase.TestMethod.Method.Name, this.TestCase.TestMethod.Method.GetBindingFlags());

                    var obj = method.IsStatic ? null : Activator.CreateInstance(type, this.constructorArguments);
                    var result = method.Invoke(obj, new object[0]);
                    var task = result as Task;
                    if (task != null)
                    {
                        await task;
                    }
                });

                var stepFailed = false;
                StepTestCase failedStep = null;
                var interceptingBus = new DelegatingMessageBus(
                    this.MessageBus,
                    message =>
                    {
                        if (message is ITestFailed)
                        {
                            stepFailed = true;
                        }
                    });

                foreach (var testCase in CurrentScenario.ExtractSteps()
                    .Select(step => new StepTestCase(this.TestCase.TestMethod, step)))
                {
                    if (failedStep != null)
                    {
                        var message = string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to execute preceding step \"{0}\".",
                            failedStep.Step.Name);

                        var failingTestCase = new LambdaTestCase(
                            this.TestCase.TestMethod,
                            () =>
                            {
                                throw new InvalidOperationException(message);
                            });

                        await failingTestCase.RunAsync(
                            this.MessageBus, this.constructorArguments, this.Aggregator, this.CancellationTokenSource);

                        continue;
                    }

                    summary.Aggregate(
                        await testCase.RunAsync(
                            interceptingBus, this.constructorArguments, this.Aggregator, this.CancellationTokenSource));

                    if (stepFailed)
                    {
                        failedStep = testCase;
                    }
                }
            }
            catch (Exception ex)
            {
                summary.Failed++;
                if (!this.MessageBus.QueueMessage(
                    new TestFailed(this.TestCase, this.displayName, timer.Total, null, ex)))
                {
                    this.CancellationTokenSource.Cancel();
                }
            }

            return summary;
        }
    }
}
