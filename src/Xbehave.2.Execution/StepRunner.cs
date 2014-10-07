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
        private readonly Step step;
        private readonly string stepName;

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
            Guard.AgainstNullArgument("methodCall", methodCall);
            Guard.AgainstNullArgument("step", step);

            if (step.Name == null)
            {
                throw new ArgumentException("The step name is null.", "step");
            }

            this.step = step;

            var provider = CultureInfo.InvariantCulture;
            string renderedStepName;
            try
            {
                renderedStepName = string.Format(provider, step.Name, testMethodArguments.Select(argument => argument ?? "null")
                    .ToArray());
            }
            catch (FormatException)
            {
                renderedStepName = step.Name;
            }

            this.stepName = string.Format(provider, "{0} {1}", this.Name, renderedStepName);
            this.DisplayName = string.Format(provider, "{0} {1}", this.DisplayName, renderedStepName);
        }

        public string StepName
        {
            get { return this.stepName; }
        }

        protected override async Task<decimal> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var timer = new ExecutionTimer();
            await aggregator.RunAsync(this.step.Body);
            return timer.Total;
        }
    }
}
