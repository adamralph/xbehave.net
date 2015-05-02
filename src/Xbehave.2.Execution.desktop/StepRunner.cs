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

    public class StepRunner : XunitTestRunner
    {
        private readonly StepDefinition step;
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public StepRunner(
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
            get
            {
                return this.disposables.Select(disposable => (Action)disposable.Dispose)
                    .Concat(this.step.Teardowns).ToArray();
            }
        }

        protected StepDefinition Step
        {
            get { return this.step; }
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            var tuple = await new StepInvoker(this.step.Body, aggregator, this.CancellationTokenSource).RunAsync();
            this.disposables.AddRange(tuple.Item2);
            return tuple.Item1;
        }
    }
}
