// <copyright file="ScenarioRunner.cs" company="xBehave.net contributors">
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
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunner : XunitTestCaseRunner
    {
        public ScenarioRunner(
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                testMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            if (!string.IsNullOrEmpty(this.SkipReason))
            {
                if (!this.MessageBus.QueueMessage(new TestSkipped(this.TestCase, this.DisplayName, this.SkipReason)))
                {
                    CancellationTokenSource.Cancel();
                }

                return new RunSummary { Skipped = 1 };
            }

            var stepFailed = false;
            var interceptingBus = new DelegatingMessageBus(
                this.MessageBus,
                message =>
                {
                    if (message is ITestFailed)
                    {
                        stepFailed = true;
                    }
                });

            var stepRunners = new List<StepRunner>();
            try
            {
                var type = Reflector.GetType(
                    this.TestCase.TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name,
                    this.TestCase.TestMethod.TestClass.Class.Name);

                var method = type.GetMethod(this.TestCase.TestMethod.Method.Name);

                var obj = method.IsStatic ? null : Activator.CreateInstance(type, this.ConstructorArguments);
                var result = method.Invoke(obj, this.TestMethodArguments);
                var task = result as Task;
                if (task != null)
                {
                    await task;
                }

                stepRunners.AddRange(CurrentScenario.ExtractSteps()
                    .Select(step => new StepRunner(
                        step.Name,
                        step.Body,
                        this.TestCase,
                        interceptingBus,
                        this.TestClass,
                        this.ConstructorArguments,
                        this.TestMethod,
                        this.TestMethodArguments,
                        this.DisplayName,
                        step.SkipReason,
                        this.Aggregator,
                        this.CancellationTokenSource)));
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

            var summary = new RunSummary();
            string failedStepName = null;
            foreach (var stepRunner in stepRunners)
            {
                if (failedStepName != null)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to execute preceding step \"{0}\".",
                        failedStepName);

                    var failFast = new LambdaTestCase(
                        this.TestCase.TestMethod,
                        () =>
                        {
                            throw new InvalidOperationException(message);
                        });

                    await failFast.RunAsync(
                        this.MessageBus, this.ConstructorArguments, this.Aggregator, this.CancellationTokenSource);

                    continue;
                }

                summary.Aggregate(await stepRunner.RunAsync());

                if (stepFailed)
                {
                    failedStepName = stepRunner.StepName;
                }
            }

            return summary;
        }
    }
}
