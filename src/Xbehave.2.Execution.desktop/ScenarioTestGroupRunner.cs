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
        private readonly IMessageBus messageBus;
        private readonly Type testClass;
        private readonly object[] constructorArguments;
        private readonly MethodInfo testMethod;
        private readonly object[] testMethodArguments;
        private readonly string skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes;
        private readonly ExceptionAggregator parentAggregator;
        private readonly CancellationTokenSource cancellationTokenSource;

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
            Guard.AgainstNullArgument("testGroup", testGroup);
            Guard.AgainstNullArgument("messageBus", messageBus);
            Guard.AgainstNullArgument("aggregator", aggregator);

            this.testGroup = testGroup;
            this.scenarioNumber = scenarioNumber;
            this.messageBus = messageBus;
            this.testClass = testClass;
            this.constructorArguments = constructorArguments;
            this.testMethod = testMethod;
            this.testMethodArguments = testMethodArguments;
            this.skipReason = skipReason;
            this.beforeAfterTestGroupAttributes = beforeAfterTestGroupAttributes;
            this.parentAggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        protected int ScenarioNumber
        {
            get { return this.scenarioNumber; }
        }

        protected IScenarioTestGroup TestGroup
        {
            get { return this.testGroup; }
        }

        protected IMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        protected Type TestClass
        {
            get { return this.testClass; }
        }

        protected IReadOnlyList<object> ConstructorArguments
        {
            get { return this.constructorArguments; }
        }

        protected MethodInfo TestMethod
        {
            get { return this.testMethod; }
        }

        protected IReadOnlyList<object> TestMethodArguments
        {
            get { return this.testMethodArguments; }
        }

        protected string SkipReason
        {
            get { return this.skipReason; }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterTestGroupAttributes
        {
            get { return this.beforeAfterTestGroupAttributes; }
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
            var runSummary = new RunSummary();

            if (!string.IsNullOrEmpty(this.skipReason))
            {
                runSummary.Total++;
                runSummary.Skipped++;
                this.messageBus.Queue(
                    new XunitTest(this.testGroup.TestCase, this.testGroup.DisplayName),
                    t => new TestSkipped(t, this.skipReason),
                    this.CancellationTokenSource);
            }
            else
            {
                var childAggregator = new ExceptionAggregator(this.parentAggregator);
                if (!childAggregator.HasExceptions)
                {
                    runSummary.Aggregate(await this.parentAggregator.RunAsync(() => this.InvokeTestGroupAsync(childAggregator)));
                }

                var exception = childAggregator.ToException();
                if (exception != null)
                {
                    runSummary.Total++;
                    runSummary.Failed++;
                    this.messageBus.Queue(
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
                    this.messageBus,
                    this.testClass,
                    this.constructorArguments,
                    this.testMethod,
                    this.testMethodArguments,
                    this.beforeAfterTestGroupAttributes,
                    aggregator,
                    this.cancellationTokenSource)
                .RunAsync();
        }
    }
}
