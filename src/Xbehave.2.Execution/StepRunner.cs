// <copyright file="StepRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Sdk;

    public class StepRunner : TestRunner<IXunitTestCase>
    {
        private readonly string stepName;
        private readonly Func<Task> stepBody;

        public StepRunner(
            string stepName,
            Func<Task> stepBody,
            IXunitTestCase testCase,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string displayName,
            string skipReason,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                displayName,
                skipReason,
                aggregator,
                cancellationTokenSource)
        {
            this.stepName = stepName;
            this.stepBody = stepBody;
        }

        public string StepName
        {
            get { return this.stepName; }
        }

        protected override async Task<decimal> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var timer = new ExecutionTimer();
            await aggregator.RunAsync(this.stepBody);
            return timer.Total;
        }
    }
}
