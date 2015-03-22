// <copyright file="StepTestRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class StepTestRunner : XunitTestRunner
    {
        private readonly Step step;
        private readonly List<Action> teardowns = new List<Action>();

        public StepTestRunner(
            Step step,
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
            Guard.AgainstNullArgument("step", step);

            this.step = step;
        }

        public IEnumerable<Action> Teardowns
        {
            get { return this.teardowns.ToArray(); }
        }

        protected Step Step
        {
            get { return this.step; }
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            var stepTestInvoker = new StepTestInvoker(this.step, aggregator, this.CancellationTokenSource);

            try
            {
                return await stepTestInvoker.RunAsync();
            }
            finally
            {
                this.teardowns.AddRange(stepTestInvoker.Teardowns);
            }
        }
    }
}
