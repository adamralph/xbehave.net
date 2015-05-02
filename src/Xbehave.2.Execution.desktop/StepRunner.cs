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

    public class StepRunner : TestRunner<IXunitTestCase>
    {
        private readonly Func<IStepContext, Task> body;
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public StepRunner(
            ITest step,
            Func<IStepContext, Task> body,
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
            this.body = body;
        }

        public IReadOnlyList<IDisposable> Disposables
        {
            get { return this.disposables; }
        }

        protected Func<IStepContext, Task> Body
        {
            get { return this.body; }
        }

        protected async override Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var tuple = await new StepInvoker(this.body, aggregator, this.CancellationTokenSource).RunAsync();
            this.disposables.AddRange(tuple.Item2);
            return Tuple.Create(tuple.Item1, string.Empty);
        }
    }
}
