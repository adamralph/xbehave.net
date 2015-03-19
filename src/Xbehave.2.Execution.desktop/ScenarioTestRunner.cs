// <copyright file="ScenarioTestRunner.cs" company="xBehave.net contributors">
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

    public class ScenarioTestRunner
    {
        private readonly int scenarioNumber;
        private readonly ITest test;
        private readonly IMessageBus messageBus;
        private readonly Type testClass;
        private readonly object[] constructorArguments;
        private readonly MethodInfo testMethod;
        private readonly object[] testMethodArguments;
        private readonly string skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;

        public ScenarioTestRunner(
            int scenarioNumber,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            this.scenarioNumber = scenarioNumber;
            this.test = test;
            this.messageBus = messageBus;
            this.testClass = testClass;
            this.constructorArguments = constructorArguments;
            this.testMethod = testMethod;
            this.testMethodArguments = testMethodArguments;
            this.skipReason = skipReason;
            this.beforeAfterAttributes = beforeAfterAttributes;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public async Task<RunSummary> RunAsync()
        {
            var runSummary = new RunSummary { Total = 1 };

            if (!string.IsNullOrEmpty(this.skipReason))
            {
                runSummary.Skipped++;
                this.messageBus.Queue(this.test, t => new TestSkipped(t, this.skipReason), this.cancellationTokenSource);
            }
            else
            {
                if (!this.aggregator.HasExceptions)
                {
                    runSummary.Time = await this.aggregator.RunAsync(() => new ScenarioTestInvoker(
                            this.scenarioNumber,
                            this.test,
                            this.messageBus,
                            this.testClass,
                            this.constructorArguments,
                            this.testMethod,
                            this.testMethodArguments,
                            this.beforeAfterAttributes,
                            this.aggregator,
                            this.cancellationTokenSource)
                        .RunAsync());
                }
                else
                {
                    this.messageBus.Queue(
                        this.test,
                        t => new TestFailed(t, 0, null, this.aggregator.ToException()),
                        this.cancellationTokenSource);
                }
            }

            return runSummary;
        }
    }
}
