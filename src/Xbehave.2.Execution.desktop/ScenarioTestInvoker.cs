// <copyright file="ScenarioTestInvoker.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioTestInvoker : TestInvoker<IXunitTestCase>
    {
        private readonly int scenarioNumber;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes;

        public ScenarioTestInvoker(
            int scenarioNumber,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                aggregator,
                cancellationTokenSource)
        {
            this.scenarioNumber = scenarioNumber;
            this.beforeAfterAttributes = beforeAfterAttributes;
        }

        public async override Task<decimal> InvokeTestMethodAsync(object testClassInstance)
        {
            var stepFailed = false;
            var interceptingBus = new DelegatingMessageBus(
                this.MessageBus,
                message =>
                {
                    if (message is ITestFailed)
                    {
                        stepFailed = true;
                    }
                });

            var stepTestRunners = new List<StepTestRunner>();
            try
            {
                await this.InvokeBackgroundMethods(testClassInstance);
                await this.TestMethod.InvokeAsync(testClassInstance, this.TestMethodArguments);
                stepTestRunners.AddRange(CurrentScenario.ExtractSteps()
                    .Select((step, index) => this.CreateStepTestRunner(interceptingBus, step, index + 1)));
            }
            catch (Exception ex)
            {
                this.MessageBus.Queue(
                    this.Test, test => new TestFailed(test, 0, null, ex.Unwrap()), this.CancellationTokenSource);

                return 0;
            }

            var summary = new RunSummary();
            string failedStepName = null;
            foreach (var stepTestRunner in stepTestRunners)
            {
                if (failedStepName != null)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture, "Failed to execute preceding step \"{0}\".", failedStepName);

                    this.MessageBus.Queue(
                        new XunitTest(this.TestCase, stepTestRunner.TestDisplayName),
                        test => new TestFailed(test, 0, string.Empty, new InvalidOperationException(message)),
                        this.CancellationTokenSource);

                    continue;
                }

                summary.Aggregate(await stepTestRunner.RunAsync());

                if (stepFailed)
                {
                    failedStepName = stepTestRunner.StepDisplayName;
                }
            }

            var teardowns = stepTestRunners.SelectMany(runner => runner.Teardowns).ToArray();
            if (teardowns.Any())
            {
                var timer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();

                foreach (var teardown in teardowns.Reverse())
                {
                    timer.Aggregate(() => teardownAggregator.Run(() => teardown()));
                }

                if (teardownAggregator.HasExceptions)
                {
                    this.MessageBus.Queue(
                        new XunitTest(
                            TestCase,
                            GetDisplayName(this.DisplayName, this.scenarioNumber, stepTestRunners.Count + 1, "(Teardown)")),
                        test => new TestFailed(test, 0, null, teardownAggregator.ToException()),
                        this.CancellationTokenSource);
                }

                summary.Time += timer.Total;
            }

            return summary.Time;
        }

        private static string GetDisplayName(string scenarioName, int scenarioNumber, int stepNumber, string stepName)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}.{2}] {3}",
                scenarioName,
                scenarioNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepName);
        }

        private async Task InvokeBackgroundMethods(object testClassInstance)
        {
            CurrentScenario.AddingBackgroundSteps = true;
            try
            {
                foreach (var backgroundMethod in this.TestCase.TestMethod.Method.Type
                    .GetMethods(false)
                    .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                    .Select(method => method.ToRuntimeMethod()))
                {
                    await backgroundMethod.InvokeAsync(testClassInstance, null);
                }
            }
            finally
            {
                CurrentScenario.AddingBackgroundSteps = false;
            }
        }

        private StepTestRunner CreateStepTestRunner(IMessageBus messageBus, Step step, int stepNumber)
        {
            string stepName;
            try
            {
                stepName = string.Format(
                    CultureInfo.InvariantCulture,
                    step.Name,
                    this.TestMethodArguments.Select(argument => argument ?? "null").ToArray());
            }
            catch (FormatException)
            {
                stepName = step.Name;
            }

            return new StepTestRunner(
                stepName,
                step,
                new XunitTest(this.TestCase, GetDisplayName(this.DisplayName, this.scenarioNumber, stepNumber, stepName)),
                messageBus,
                this.TestClass,
                this.ConstructorArguments,
                this.TestMethod,
                this.TestMethodArguments,
                step.SkipReason,
                this.beforeAfterAttributes,
                this.Aggregator,
                this.CancellationTokenSource);
        }
    }
}
