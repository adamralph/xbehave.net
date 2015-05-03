// <copyright file="ScenarioRunner.cs" company="xBehave.net contributors">
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
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunner : XunitTestRunner, IDisposable
    {
        public ScenarioRunner(
            ITest scenario,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            MethodInfo scenarioMethod,
            object[] scenarioMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                scenario,
                messageBus,
                scenarioClass,
                constructorArguments,
                scenarioMethod,
                scenarioMethodArguments,
                skipReason,
                beforeAfterScenarioAttributes,
                aggregator,
                cancellationTokenSource)
        {
        }

        ~ScenarioRunner()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.TestMethodArguments != null)
            {
                foreach (var disposable in this.TestMethodArguments.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
            }
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            return await new ScenarioInvoker(
                    this.Test,
                    this.MessageBus,
                    this.TestClass,
                    this.ConstructorArguments,
                    this.TestMethod,
                    this.TestMethodArguments,
                    this.BeforeAfterAttributes,
                    aggregator,
                    this.CancellationTokenSource)
                .RunAsync();
        }
    }
}
