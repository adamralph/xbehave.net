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
    using Xbehave.Execution.Extensions;
    using Xbehave.Sdk;
    using Xunit.Sdk;

    public class ScenarioRunner : IDisposable
    {
        private readonly IScenario scenario;
        private readonly IMessageBus messageBus;
        private readonly Type scenarioClass;
        private readonly object[] constructorArguments;
        private readonly MethodInfo scenarioMethod;
        private readonly object[] scenarioMethodArguments;
        private readonly string skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes;
        private readonly ExceptionAggregator parentAggregator;
        private readonly CancellationTokenSource cancellationTokenSource;

        public ScenarioRunner(
            IScenario scenario,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            MethodInfo scenarioMethod,
            object[] scenarioMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("scenario", scenario);
            Guard.AgainstNullArgument("messageBus", messageBus);
            Guard.AgainstNullArgument("aggregator", aggregator);

            this.scenario = scenario;
            this.messageBus = messageBus;
            this.scenarioClass = scenarioClass;
            this.constructorArguments = constructorArguments;
            this.scenarioMethod = scenarioMethod;
            this.scenarioMethodArguments = scenarioMethodArguments;
            this.skipReason = skipReason;
            this.beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
            this.parentAggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        ~ScenarioRunner()
        {
            this.Dispose(false);
        }

        public async Task<RunSummary> RunAsync()
        {
            if (!string.IsNullOrEmpty(this.skipReason))
            {
                this.messageBus.Queue(
                    this.scenario, test => new TestSkipped(test, this.skipReason), this.cancellationTokenSource);

                return new RunSummary { Total = 1, Skipped = 1 };
            }
            else
            {
                var summary = new RunSummary();
                var output = string.Empty;
                var childAggregator = new ExceptionAggregator(this.parentAggregator);
                if (!childAggregator.HasExceptions)
                {
                    var tuple = await childAggregator.RunAsync(() => this.InvokeScenarioAsync(childAggregator));
                    summary.Aggregate(tuple.Item1);
                    output = tuple.Item2;
                }

                var exception = childAggregator.ToException();
                if (exception != null)
                {
                    summary.Total++;
                    summary.Failed++;
                    this.messageBus.Queue(
                        this.scenario,
                        test => new TestFailed(test, summary.Time, output, exception),
                        this.cancellationTokenSource);
                }
                else if (summary.Total == 0)
                {
                    summary.Total++;
                    this.messageBus.Queue(
                        this.scenario, test => new TestPassed(test, summary.Time, output), this.cancellationTokenSource);
                }

                return summary;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.scenarioMethodArguments != null)
            {
                foreach (var disposable in this.scenarioMethodArguments.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
            }
        }

        private async Task<Tuple<RunSummary, string>> InvokeScenarioAsync(ExceptionAggregator aggregator)
        {
            var output = string.Empty;
            var testOutputHelper = this.constructorArguments.OfType<TestOutputHelper>().FirstOrDefault();
            if (testOutputHelper != null)
            {
                testOutputHelper.Initialize(this.messageBus, this.scenario);
            }

            var summary = await this.InvokeScenarioMethodAsync(aggregator);

            if (testOutputHelper != null)
            {
                output = testOutputHelper.Output;
                testOutputHelper.Uninitialize();
            }

            return Tuple.Create(summary, output);
        }

        private async Task<RunSummary> InvokeScenarioMethodAsync(ExceptionAggregator aggregator)
        {
            return await new ScenarioInvoker(
                    this.scenario,
                    this.messageBus,
                    this.scenarioClass,
                    this.constructorArguments,
                    this.scenarioMethod,
                    this.scenarioMethodArguments,
                    this.beforeAfterScenarioAttributes,
                    aggregator,
                    this.cancellationTokenSource)
                .RunAsync();
        }
    }
}
