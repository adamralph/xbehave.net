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
        private readonly StepDefinition stepDefinition;
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        // TODO: stop taking StepDefinition as a param
        public StepRunner(
            StepDefinition stepDefinition,
            ITest step,
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
            this.stepDefinition = stepDefinition;
        }

        public IReadOnlyList<Action> Teardowns
        {
            get
            {
                return this.disposables.Select(disposable => (Action)disposable.Dispose)
                    .Concat(this.stepDefinition.Teardowns).ToArray();
            }
        }

        protected StepDefinition StepDefinition
        {
            get { return this.stepDefinition; }
        }

        protected async override Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var tuple = await new StepInvoker(this.stepDefinition.Body, aggregator, this.CancellationTokenSource).RunAsync();
            this.disposables.AddRange(tuple.Item2);
            return Tuple.Create(tuple.Item1, string.Empty);
        }
    }
}
