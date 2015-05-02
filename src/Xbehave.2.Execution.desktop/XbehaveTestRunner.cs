// <copyright file="XbehaveTestRunner.cs" company="xBehave.net contributors">
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

    public class XbehaveTestRunner : XunitTestRunner
    {
        private readonly StepDefinition step;
        private readonly List<Action> teardowns = new List<Action>();

        public XbehaveTestRunner(
            StepDefinition step,
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
            this.step = step;
        }

        public IReadOnlyList<Action> Teardowns
        {
            get { return this.teardowns.ToArray(); }
        }

        protected StepDefinition Step
        {
            get { return this.step; }
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            var tuple = await new XbehaveTestInvoker(this.step, aggregator, this.CancellationTokenSource).RunAsync();
            this.teardowns.AddRange(tuple.Item2);
            return tuple.Item1;
        }
    }
}
