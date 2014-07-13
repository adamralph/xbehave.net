// <copyright file="StepTestCase.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class StepTestCase : XunitTestCase
    {
        private readonly Step step;

        public StepTestCase(ITestMethod testMethod, Step step)
            : base(testMethod)
        {
            if (step == null)
            {
                throw new ArgumentNullException("step");
            }

            this.step = step;
        }

        public Step Step
        {
            get { return this.step; }
        }

        public override async Task<RunSummary> RunAsync(
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return await new StepTestCaseRunner(this, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }
    }
}
