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
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes;

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
            this.Test = test;
            this.MessageBus = messageBus;
            this.TestClass = testClass;
            this.ConstructorArguments = constructorArguments;
            this.TestMethod = testMethod;
            this.TestMethodArguments = testMethodArguments;
            this.SkipReason = skipReason;
            this.beforeAfterAttributes = beforeAfterAttributes;
            this.Aggregator = aggregator;
            this.CancellationTokenSource = cancellationTokenSource;
        }

        protected int ScenarioNumber
        {
            get { return this.scenarioNumber; }
        }

        protected ITest Test { get; set; }

        protected IMessageBus MessageBus { get; set; }

        protected Type TestClass { get; set; }

        protected object[] ConstructorArguments { get; set; }

        protected MethodInfo TestMethod { get; set; }

        protected object[] TestMethodArguments { get; set; }

        protected string SkipReason { get; set; }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterAttributes
        {
            get { return this.beforeAfterAttributes; }
        }

        protected ExceptionAggregator Aggregator { get; set; }

        protected CancellationTokenSource CancellationTokenSource { get; set; }

        public async Task<RunSummary> RunAsync()
        {
            var runSummary = new RunSummary { Total = 1 };

            if (!string.IsNullOrEmpty(this.SkipReason))
            {
                runSummary.Skipped++;
                this.MessageBus.Queue(this.Test, t => new TestSkipped(t, this.SkipReason), this.CancellationTokenSource);
            }
            else
            {
                var output = string.Empty;
                var aggregator = new ExceptionAggregator(this.Aggregator);
                if (!aggregator.HasExceptions)
                {
                    var tuple = await this.Aggregator.RunAsync(() => this.InvokeTestAsync(aggregator));
                    runSummary.Time = tuple.Item1;
                    output = tuple.Item2;
                }

                var exception = aggregator.ToException();
                if (exception != null)
                {
                    runSummary.Failed++;
                    this.MessageBus.Queue(
                        this.Test,
                        t => new TestFailed(t, runSummary.Time, output, exception),
                        this.CancellationTokenSource);
                }
            }

            return runSummary;
        }

        protected virtual async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            // NOTE (adamralph): as in XunitTestRunner, we use a ScenarioOutputHelper here for output
            return Tuple.Create(await this.InvokeTestMethodAsync(), string.Empty);
        }

        protected virtual async Task<decimal> InvokeTestMethodAsync()
        {
            return await new ScenarioTestInvoker(
                    this.ScenarioNumber,
                    this.Test,
                    this.MessageBus,
                    this.TestClass,
                    this.ConstructorArguments,
                    this.TestMethod,
                    this.TestMethodArguments,
                    this.BeforeAfterAttributes,
                    this.Aggregator,
                    this.CancellationTokenSource)
                .RunAsync();
        }
    }
}
