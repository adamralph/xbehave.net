// <copyright file="ScenarioRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunner : XunitTestRunner
    {
        private readonly int scenarioNumber;

        public ScenarioRunner(
            int scenarioNumber,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                skipReason,
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            this.scenarioNumber = scenarioNumber;
        }

        // NOTE (adamralph): lifted from Xunit.Sdk.TestRunner.RunAsync() with removal of sending of TestPassed
        public new async Task<RunSummary> RunAsync()
        {
            var runSummary = new RunSummary { Total = 1 };
            var output = string.Empty;

            if (!MessageBus.QueueMessage(new TestStarting(Test)))
            {
                CancellationTokenSource.Cancel();
            }
            else
            {
                this.AfterTestStarting();

                if (!string.IsNullOrEmpty(this.SkipReason))
                {
                    runSummary.Skipped++;

                    if (!MessageBus.QueueMessage(new TestSkipped(this.Test, this.SkipReason)))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }
                else
                {
                    var aggregator = new ExceptionAggregator(this.Aggregator);

                    if (!aggregator.HasExceptions)
                    {
                        var tuple = await aggregator.RunAsync(() => this.InvokeTestAsync(aggregator));
                        runSummary.Time = tuple.Item1;
                        output = tuple.Item2;
                    }

                    var exception = aggregator.ToException();

                    if (exception != null)
                    {
                        var testResult = new TestFailed(this.Test, runSummary.Time, output, exception);
                        runSummary.Failed++;
                        if (!this.CancellationTokenSource.IsCancellationRequested)
                        {
                            if (!this.MessageBus.QueueMessage(testResult))
                            {
                                this.CancellationTokenSource.Cancel();
                            }
                        }
                    }
                }

                this.Aggregator.Clear();
                this.BeforeTestFinished();

                if (this.Aggregator.HasExceptions)
                {
                    if (!this.MessageBus.QueueMessage(new TestCleanupFailure(this.Test, this.Aggregator.ToException())))
                    {
                        this.CancellationTokenSource.Cancel();
                    }
                }
            }

            if (!MessageBus.QueueMessage(new TestFinished(Test, runSummary.Time, output)))
            {
                this.CancellationTokenSource.Cancel();
            }

            return runSummary;
        }

        protected async override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            return await new ScenarioInvoker(
                    this.scenarioNumber,
                    this.Test,
                    this.MessageBus,
                    this.TestClass,
                    this.ConstructorArguments,
                    this.TestMethod,
                    this.TestMethodArguments,
                    this.BeforeAfterAttributes,
                    aggregator,
                    this.CancellationTokenSource)
                .RunAsync();
        }
    }
}
