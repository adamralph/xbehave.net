// <copyright file="XbehaveTestCaseRunner.cs" company="xBehave.net contributors">
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

    public class XbehaveTestCaseRunner : XunitTestCaseRunner
    {
        private static readonly object[] noArguments = new object[0];

        private readonly IMessageSink diagnosticMessageSink;
        private readonly ExceptionAggregator cleanupAggregator = new ExceptionAggregator();
        private readonly List<XbehaveTestGroup> testGroups = new List<XbehaveTestGroup>();
        private Exception dataDiscoveryException;

        public XbehaveTestCaseRunner(
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
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        protected IMessageSink DiagnosticMessageSink
        {
            get { return this.diagnosticMessageSink; }
        }

        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute)).ToList();
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var discoverer =
                        ExtensibilityPointFactory.GetDataDiscoverer(this.diagnosticMessageSink, discovererAttribute);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, TestCase.TestMethod.Method))
                    {
                        var testGroup = new XbehaveTestGroup(
                            this.TestCase,
                            this.DisplayName,
                            this.TestClass,
                            this.TestMethod,
                            dataRow,
                            this.SkipReason,
                            this.BeforeAfterAttributes);

                        this.testGroups.Add(testGroup);
                    }
                }

                if (!this.testGroups.Any())
                {
                    var testGroup = new XbehaveTestGroup(
                        this.TestCase,
                        this.DisplayName,
                        this.TestClass,
                        this.TestMethod,
                        noArguments,
                        this.SkipReason,
                        this.BeforeAfterAttributes);

                    this.testGroups.Add(testGroup);
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
            foreach (var testGroup in this.testGroups)
            {
                summary.Aggregate(await testGroup.RunAsync(
                        this.diagnosticMessageSink,
                        this.MessageBus,
                        this.ConstructorArguments,
                        new ExceptionAggregator(this.Aggregator),
                        this.CancellationTokenSource));
            }

            // Run the cleanup here so we can include cleanup time in the run summary,
            // but save any exceptions so we can surface them during the cleanup phase,
            // so they get properly reported as test case cleanup failures.
            var timer = new ExecutionTimer();
            foreach (var testGroup in this.testGroups)
            {
                timer.Aggregate(() => this.cleanupAggregator.Run(() => testGroup.Dispose()));
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
