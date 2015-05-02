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

    // TODO: stop inheriting from XunitTestRunner
    public class StepRunner : XunitTestRunner
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

        // TODO: change name
        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            var tuple = await new StepInvoker(this.stepDefinition.Body, aggregator, this.CancellationTokenSource).RunAsync();
            this.disposables.AddRange(tuple.Item2);
            return tuple.Item1;
        }
    }
}
