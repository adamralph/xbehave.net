// <copyright file="StepRunner.cs" company="xBehave.net contributors">
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

    public class StepRunner : XbehaveTestRunner
    {
        private readonly string stepDisplayName;
        private readonly Step step;
        private readonly List<Action> teardowns = new List<Action>();

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
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            Guard.AgainstNullArgument("step", step);

            this.stepDisplayName = stepDisplayName;
            this.step = step;
        }

        public string StepDisplayName
        {
            get { return this.stepDisplayName; }
        }

        public IEnumerable<Action> Teardowns
        {
            get { return this.teardowns.ToArray(); }
        }

        protected override async Task<decimal> InvokeDelegatesAsync(ExceptionAggregator aggregator)
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
