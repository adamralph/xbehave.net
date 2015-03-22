// <copyright file="ScenarioOutlineTestCaseRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioOutlineTestCaseRunner : XunitTestCaseRunner
    {
        private static readonly object[] noArguments = new object[0];

        private readonly ExceptionAggregator cleanupAggregator = new ExceptionAggregator();
        private readonly List<ScenarioTestGroup> scenarioTestGroups = new List<ScenarioTestGroup>();
        private Exception dataDiscoveryException;

        public ScenarioOutlineTestCaseRunner(
            IMessageSink diagnosticMessageSink,
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                noArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
            this.DiagnosticMessageSink = diagnosticMessageSink;
        }

        protected IMessageSink DiagnosticMessageSink { get; set; }

        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            var scenarioNumber = 1;
            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute)).ToList();
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var discoverer =
                        ExtensibilityPointFactory.GetDataDiscoverer(this.DiagnosticMessageSink, discovererAttribute);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, TestCase.TestMethod.Method))
                    {
                        var scenarioTestGroup = new ScenarioTestGroup(
                            this.TestCase,
                            this.DisplayName,
                            scenarioNumber++,
                            this.TestClass,
                            this.TestMethod,
                            dataRow,
                            this.SkipReason,
                            this.BeforeAfterAttributes);

                        this.scenarioTestGroups.Add(scenarioTestGroup);
                    }
                }

                if (!this.scenarioTestGroups.Any())
                {
                    var scenarioTestGroup = new ScenarioTestGroup(
                        this.TestCase,
                        this.DisplayName,
                        1,
                        this.TestClass,
                        this.TestMethod,
                        noArguments,
                        this.SkipReason,
                        this.BeforeAfterAttributes);

                    this.scenarioTestGroups.Add(scenarioTestGroup);
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
                    new XunitTest(TestCase, DisplayName),
                    test => new TestFailed(test, 0, null, this.dataDiscoveryException.Unwrap()),
                    this.CancellationTokenSource);

                return new RunSummary { Total = 1, Failed = 1 };
            }

            var summary = new RunSummary();
            foreach (var scenarioTestGroup in this.scenarioTestGroups)
            {
                summary.Aggregate(await scenarioTestGroup.RunAsync(
                        this.DiagnosticMessageSink,
                        this.MessageBus,
                        this.ConstructorArguments,
                        new ExceptionAggregator(this.Aggregator),
                        this.CancellationTokenSource));
            }

            // Run the cleanup here so we can include cleanup time in the run summary,
            // but save any exceptions so we can surface them during the cleanup phase,
            // so they get properly reported as test case cleanup failures.
            var timer = new ExecutionTimer();
            foreach (var scenarioTestGroup in this.scenarioTestGroups)
            {
                timer.Aggregate(() => this.cleanupAggregator.Run(() => scenarioTestGroup.Dispose()));
            }

            summary.Time += timer.Total;
            return summary;
        }

        protected override Task BeforeTestCaseFinishedAsync()
        {
            Aggregator.Aggregate(this.cleanupAggregator);

            return base.BeforeTestCaseFinishedAsync();
        }
    }
}
