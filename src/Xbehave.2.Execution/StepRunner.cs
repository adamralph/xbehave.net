// <copyright file="StepRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Sdk;

    public class StepRunner : TestRunner<IXunitTestCase>
    {
        private readonly string stepName;
        private readonly Func<Task> body;

        public StepRunner(
            Step step,
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
            Guard.AgainstNullArgument("step", step);

            try
            {
                this.stepName = string.Format(CultureInfo.InvariantCulture, step.Name, testMethodArguments);
            }
            catch (FormatException)
            {
                this.stepName = step.Name;
            }

            this.body = step.Body;
            this.DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.DisplayName, this.stepName);
        }

        public string StepName
        {
            get { return this.stepName; }
        }

        protected override async Task<decimal> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var timer = new ExecutionTimer();
            await aggregator.RunAsync(this.body);
            return timer.Total;
        }
    }
}
