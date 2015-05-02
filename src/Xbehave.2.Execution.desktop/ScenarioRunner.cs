// <copyright file="ScenarioRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunner
    {
        private readonly ITest scenario;
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

        protected ITest Scenario
        {
            get { return this.scenario; }
        }

        protected IMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        protected Type ScenarioClass
        {
            get { return this.scenarioClass; }
        }

        protected IReadOnlyList<object> ConstructorArguments
        {
            get { return this.constructorArguments; }
        }

        protected MethodInfo ScenarioMethod
        {
            get { return this.scenarioMethod; }
        }

        protected IReadOnlyList<object> ScenarioMethodArguments
        {
            get { return this.scenarioMethodArguments; }
        }

        protected string SkipReason
        {
            get { return this.skipReason; }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterScenarioAttributes
        {
            get { return this.beforeAfterScenarioAttributes; }
        }

        protected ExceptionAggregator Aggregator
        {
            get { return this.parentAggregator; }
        }

        protected CancellationTokenSource CancellationTokenSource
        {
            get { return this.cancellationTokenSource; }
        }

        public async Task<RunSummary> RunAsync()
        {
            if (!string.IsNullOrEmpty(this.skipReason))
            {
                this.messageBus.Queue(
                    new Step(this.scenario, this.scenario.DisplayName),
                    t => new TestSkipped(t, this.skipReason),
                    this.CancellationTokenSource);

                return new RunSummary { Total = 1, Skipped = 1 };
            }
            else
            {
                var summary = new RunSummary();
                var childAggregator = new ExceptionAggregator(this.parentAggregator);
                if (!childAggregator.HasExceptions)
                {
                    summary.Aggregate(await childAggregator.RunAsync(() => this.InvokeScenarioMethodAsync(childAggregator)));
                }

                var exception = childAggregator.ToException();
                if (exception != null)
                {
                    summary.Total++;
                    summary.Failed++;
                    this.messageBus.Queue(
                        new Step(this.scenario, this.scenario.DisplayName),
                        t => new TestFailed(t, summary.Time, string.Empty, exception),
                        this.CancellationTokenSource);
                }
                else if (summary.Total == 0)
                {
                    summary.Total++;
                    this.messageBus.Queue(
                        new Step(this.scenario, this.scenario.DisplayName),
                        test => new TestPassed(test, summary.Time, null),
                        this.cancellationTokenSource);
                }

                return summary;
            }
        }

        protected virtual async Task<RunSummary> InvokeScenarioMethodAsync(ExceptionAggregator aggregator)
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
