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

    public class ScenarioRunner
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
                    summary.Aggregate(await childAggregator.RunAsync(() => this.InvokeScenarioAsync(childAggregator)));
                }

                var exception = childAggregator.ToException();
                if (exception != null)
                {
                    summary.Total++;
                    summary.Failed++;
                    this.messageBus.Queue(
                        this.scenario,
                        test => new TestFailed(test, summary.Time, string.Empty, exception),
                        this.cancellationTokenSource);
                }
                else if (summary.Total == 0)
                {
                    summary.Total++;
                    this.messageBus.Queue(
                        this.scenario, test => new TestPassed(test, summary.Time, string.Empty), this.cancellationTokenSource);
                }

                return summary;
            }
        }

        private async Task<RunSummary> InvokeScenarioAsync(ExceptionAggregator aggregator)
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
