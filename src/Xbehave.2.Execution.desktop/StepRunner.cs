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

    public class StepRunner : TestRunner<IXunitTestCase>
    {
        private readonly IStep step;
        private readonly Func<IStepContext, Task> body;
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private ExecutionContext executionContext;

        public StepRunner(
            IStep step,
            Func<IStepContext, Task> body,
            ExecutionContext executionContext,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            MethodInfo scenarioMethod,
            object[] scenarioMethodArguments,
            string skipReason,
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
                aggregator,
                cancellationTokenSource)
        {
            this.step = step;
            this.body = body;
            this.executionContext = executionContext;
        }

        public IReadOnlyList<IDisposable> Disposables
        {
            get { return this.disposables; }
        }

        public ExecutionContext ExecutionContext
        {
            get { return this.executionContext; }
        }

        protected async override Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var tuple = await new StepInvoker(this.step, this.body, aggregator, this.CancellationTokenSource).RunAsync(this.executionContext);
            this.disposables.AddRange(tuple.Item2);
            this.executionContext = tuple.Item3;
            return Tuple.Create(tuple.Item1, string.Empty);
        }
    }
}
