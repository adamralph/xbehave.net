// <copyright file="ScenarioOutlineRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Extensions;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioOutlineRunner : XunitTestCaseRunner
    {
        private static readonly object[] noArguments = new object[0];

        private readonly IMessageSink diagnosticMessageSink;
        private readonly ScenarioRunnerFactory scenarioRunnerFactory;
        private readonly ExceptionAggregator cleanupAggregator = new ExceptionAggregator();
        private readonly List<ScenarioRunner> scenarioRunners = new List<ScenarioRunner>();
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private Exception dataDiscoveryException;

        public ScenarioOutlineRunner(
            IMessageSink diagnosticMessageSink,
            IXunitTestCase scenarioOutline,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                scenarioOutline,
                displayName,
                skipReason,
                constructorArguments,
                noArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
            this.scenarioRunnerFactory = new ScenarioRunnerFactory(
                this.TestCase,
                this.DisplayName,
                this.MessageBus,
                this.TestClass,
                this.ConstructorArguments,
                this.SkipReason,
                this.BeforeAfterAttributes,
                this.Aggregator,
                this.CancellationTokenSource);
        }

        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            try
            {
                var dataAttributes = this.TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute)).ToList();
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var discoverer =
                        ExtensibilityPointFactory.GetDataDiscoverer(this.diagnosticMessageSink, discovererAttribute);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, this.TestCase.TestMethod.Method))
                    {
                        this.disposables.AddRange(dataRow.OfType<IDisposable>());
                        this.scenarioRunners.Add(this.scenarioRunnerFactory.Create(dataRow));
                    }
                }

                if (!this.scenarioRunners.Any())
                {
                    this.scenarioRunners.Add(this.scenarioRunnerFactory.Create(noArguments));
                }
            }
            catch (Exception ex)
            {
                this.dataDiscoveryException = ex;
            }
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            if (this.dataDiscoveryException != null)
            {
                this.MessageBus.Queue(
                    new XunitTest(this.TestCase, this.DisplayName),
                    test => new TestFailed(test, 0, null, this.dataDiscoveryException.Unwrap()),
                    this.CancellationTokenSource);

                return new RunSummary { Total = 1, Failed = 1 };
            }

            var summary = new RunSummary();
            foreach (var scenarioRunner in this.scenarioRunners)
            {
                summary.Aggregate(await scenarioRunner.RunAsync());
            }

            // Run the cleanup here so we can include cleanup time in the run summary,
            // but save any exceptions so we can surface them during the cleanup phase,
            // so they get properly reported as test case cleanup failures.
            var timer = new ExecutionTimer();
            foreach (var disposable in this.disposables)
            {
                timer.Aggregate(() => this.cleanupAggregator.Run(() => disposable.Dispose()));
            }

            summary.Time += timer.Total;
            return summary;
        }

        protected override Task BeforeTestCaseFinishedAsync()
        {
            this.Aggregator.Aggregate(this.cleanupAggregator);

            return base.BeforeTestCaseFinishedAsync();
        }
    }
}
