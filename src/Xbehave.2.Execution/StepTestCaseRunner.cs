// <copyright file="StepTestCaseRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
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

            if (this.Queue(new TestStarting(TestCase, TestCase.DisplayName)))
            {
                try
                {
                    await timer.AggregateAsync(() => TestCase.Step.RunAsync());
                    this.Queue(new TestPassed(TestCase, TestCase.DisplayName, timer.Total, null));
                }
                catch (Exception ex)
                {
                    summary.Failed++;
                    this.Queue(new TestFailed(TestCase, TestCase.DisplayName, timer.Total, null, ex));
                }
            }

            this.Queue(new TestFinished(TestCase, TestCase.DisplayName, timer.Total, null));
            summary.Time = timer.Total;
            return summary;
        }

        private bool Queue(IMessageSinkMessage message)
        {
            if (!MessageBus.QueueMessage(message))
            {
                CancellationTokenSource.Cancel();
                return false;
            }

            return true;
        }
    }
}
