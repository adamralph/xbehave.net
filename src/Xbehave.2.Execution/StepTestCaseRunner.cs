// <copyright file="StepTestCaseRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Sdk;

    public class StepTestCaseRunner : TestCaseRunner<StepTestCase>
    {
        public StepTestCaseRunner(
            StepTestCase testCase,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var summary = new RunSummary { Total = 1 };
            var timer = new ExecutionTimer();

            if (!MessageBus.QueueMessage(new TestStarting(TestCase, TestCase.DisplayName)))
            {
                CancellationTokenSource.Cancel();
            }
            else
            {
                try
                {
                    await timer.AggregateAsync(() => TestCase.Step.RunAsync());

                    if (!MessageBus.QueueMessage(new TestPassed(TestCase, TestCase.DisplayName, timer.Total, null)))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }
                catch (Exception ex)
                {
                    summary.Failed++;
                    if (!MessageBus.QueueMessage(new TestFailed(TestCase, TestCase.DisplayName, timer.Total, null, ex)))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }
            }

            if (!MessageBus.QueueMessage(new TestFinished(TestCase, TestCase.DisplayName, timer.Total, null)))
            {
                CancellationTokenSource.Cancel();
            }

            summary.Time = timer.Total;
            return summary;
        }
    }
}
