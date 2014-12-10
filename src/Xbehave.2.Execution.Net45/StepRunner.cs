// <copyright file="StepRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class StepRunner : TestRunner<IXunitTestCase>
    {
        private readonly string stepDisplayName;
        private readonly Step step;
        private readonly List<Action> teardowns = new List<Action>();
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes;

        public StepRunner(
            string stepDisplayName,
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
                aggregator,
                cancellationTokenSource)
        {
            Guard.AgainstNullArgument("step", step);

            this.stepDisplayName = stepDisplayName;
            this.step = step;
            this.beforeAfterAttributes = beforeAfterAttributes;
        }

        public string StepDisplayName
        {
            get { return this.stepDisplayName; }
        }

        public IEnumerable<Action> Teardowns
        {
            get { return this.teardowns.ToArray(); }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterAttributes
        {
            get { return this.beforeAfterAttributes; }
        }

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var output = string.Empty;
            var testOutputHelper = ConstructorArguments.OfType<TestOutputHelper>().FirstOrDefault();
            if (testOutputHelper != null)
            {
                testOutputHelper.Initialize(this.MessageBus, this.Test);
            }

            var executionTime = await this.InvokeDelegatesAsync(aggregator);

            if (testOutputHelper != null)
            {
                output = testOutputHelper.Output;
                testOutputHelper.Uninitialize();
            }

            return Tuple.Create(executionTime, output);
        }

        protected virtual async Task<decimal> InvokeDelegatesAsync(ExceptionAggregator aggregator)
        {
            var invoker = new StepInvoker(
                this.DisplayName,
                this.step,
                this.Test,
                this.MessageBus,
                this.TestClass,
                this.ConstructorArguments,
                this.TestMethod,
                this.TestMethodArguments,
                this.BeforeAfterAttributes,
                aggregator,
                this.CancellationTokenSource);

            try
            {
                return await invoker.RunAsync();
            }
            finally
            {
                this.teardowns.AddRange(invoker.Teardowns);
            }
        }
    }
}
