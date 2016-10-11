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

        public StepRunner(
            IStep step,
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
            this.step = step;
            this.body = body;
        }

        public IReadOnlyList<IDisposable> Disposables
        {
            get { return this.disposables; }
        }

        protected async override Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var output = string.Empty;

            TestOutputHelper testOutputHelper = null;
            foreach (object obj in this.ConstructorArguments)
            {
                testOutputHelper = obj as TestOutputHelper;
                if (testOutputHelper != null)
                {
                    break;
                }
            }

            if (testOutputHelper != null)
            {
                testOutputHelper.Initialize(this.MessageBus, this.Test);
            }

            var tuple = await new StepInvoker(this.step, this.body, aggregator, this.CancellationTokenSource).RunAsync();
            this.disposables.AddRange(tuple.Item2);

            if (testOutputHelper != null)
            {
                output = testOutputHelper.Output;
                testOutputHelper.Uninitialize();
            }

            return Tuple.Create(tuple.Item1, output);
        }
    }
}
