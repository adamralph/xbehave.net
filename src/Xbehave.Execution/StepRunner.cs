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
    using Xunit.Sdk;

    public class StepRunner : XunitTestRunner
    {
        private readonly IStep step;
        private readonly Func<IStepContext, Task> body;
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public StepRunner(
            IStep step,
            Func<IStepContext, Task> body,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            MethodInfo scenarioMethod,
            object[] scenarioMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                step,
                messageBus,
                scenarioClass,
                constructorArguments,
                scenarioMethod,
                scenarioMethodArguments,
                skipReason,
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            this.step = step;
            this.body = body;
        }

        public IReadOnlyList<IDisposable> Disposables
        {
            get { return this.disposables; }
        }

        protected async override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            var tuple = await new StepInvoker(this.step, this.body, aggregator, this.CancellationTokenSource).RunAsync();
            this.disposables.AddRange(tuple.Item2);
            return tuple.Item1;
        }
    }
}
