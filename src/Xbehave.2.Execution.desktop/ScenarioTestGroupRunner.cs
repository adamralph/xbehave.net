// <copyright file="ScenarioTestGroupRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Sdk;

    public class ScenarioTestGroupRunner
    {
        private readonly int scenarioNumber;
        private readonly IScenarioTestGroup testGroup;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes;

        public ScenarioTestGroupRunner(
            int scenarioNumber,
            IScenarioTestGroup testGroup,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            this.testGroup = testGroup;
            this.scenarioNumber = scenarioNumber;
            this.MessageBus = messageBus;
            this.TestClass = testClass;
            this.ConstructorArguments = constructorArguments;
            this.TestMethod = testMethod;
            this.TestMethodArguments = testMethodArguments;
            this.SkipReason = skipReason;
            this.beforeAfterTestGroupAttributes = beforeAfterTestGroupAttributes;
            this.Aggregator = aggregator;
            this.CancellationTokenSource = cancellationTokenSource;
        }

        protected int ScenarioNumber
        {
            get { return this.scenarioNumber; }
        }

        protected IScenarioTestGroup TestGroup
        {
            get { return this.testGroup; }
        }

        protected IMessageBus MessageBus { get; set; }

        protected Type TestClass { get; set; }

        protected object[] ConstructorArguments { get; set; }

        protected MethodInfo TestMethod { get; set; }

        protected object[] TestMethodArguments { get; set; }

        protected string SkipReason { get; set; }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterTestGroupAttributes
        {
            get { return this.beforeAfterTestGroupAttributes; }
        }

        protected ExceptionAggregator Aggregator { get; set; }

        protected CancellationTokenSource CancellationTokenSource { get; set; }

        public async Task<RunSummary> RunAsync()
        {
            var runSummary = new RunSummary();

            if (!string.IsNullOrEmpty(this.SkipReason))
            {
                runSummary.Total++;
                runSummary.Skipped++;
                this.MessageBus.Queue(
                    new XunitTest(this.testGroup.TestCase, this.testGroup.DisplayName),
                    t => new TestSkipped(t, this.SkipReason),
                    this.CancellationTokenSource);
            }
            else
            {
                var aggregator = new ExceptionAggregator(this.Aggregator);
                if (!aggregator.HasExceptions)
                {
                    runSummary.Aggregate(await this.Aggregator.RunAsync(() => this.InvokeTestGroupAsync(aggregator)));
                }

                var exception = aggregator.ToException();
                if (exception != null)
                {
                    runSummary.Total++;
                    runSummary.Failed++;
                    this.MessageBus.Queue(
                        new XunitTest(this.testGroup.TestCase, this.testGroup.DisplayName),
                        t => new TestFailed(t, runSummary.Time, string.Empty, exception),
                        this.CancellationTokenSource);
                }
            }

            return runSummary;
        }

        protected virtual async Task<RunSummary> InvokeTestGroupAsync(ExceptionAggregator aggregator)
        {
            return await new ScenarioTestGroupInvoker(
                    this.scenarioNumber,
                    this.testGroup,
                    this.MessageBus,
                    this.TestClass,
                    this.ConstructorArguments,
                    this.TestMethod,
                    this.TestMethodArguments,
                    this.BeforeAfterTestGroupAttributes,
                    aggregator,
                    this.CancellationTokenSource)
                .RunAsync();
        }
    }
}
