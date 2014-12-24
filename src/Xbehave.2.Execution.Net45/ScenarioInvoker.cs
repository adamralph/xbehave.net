// <copyright file="ScenarioInvoker.cs" company="xBehave.net contributors">
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
    using Xbehave.Execution.Shims;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioInvoker : XunitTestInvoker
    {
        private readonly int scenarioNumber;

        public ScenarioInvoker(
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
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            this.scenarioNumber = scenarioNumber;
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

            var stepRunners = new List<StepRunner>();
            try
            {
                await this.InvokeBackgroundMethods(testClassInstance);
                await this.TestMethod.InvokeAsync(testClassInstance, this.TestMethodArguments);
                stepRunners.AddRange(CurrentScenario.ExtractSteps()
                    .Select((step, index) => this.CreateRunner(interceptingBus, step, index + 1)));
            }
            catch (Exception ex)
            {
                var test = new XunitTest(TestCase, DisplayName);

                if (!MessageBus.QueueMessage(new TestStarting(test)))
                {
                    CancellationTokenSource.Cancel();
                }
                else
                {
                    if (!MessageBus.QueueMessage(new TestFailed(test, 0, null, ex.Unwrap())))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }

                if (!MessageBus.QueueMessage(new TestFinished(test, 0, null)))
                {
                    CancellationTokenSource.Cancel();
                }

                return 0;
            }

            var summary = new RunSummary();
            string failedStepName = null;
            foreach (var stepRunner in stepRunners)
            {
                if (failedStepName != null)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture, "Failed to execute preceding step \"{0}\".", failedStepName);

                    var failFast = new LambdaTestCase(
                        this.TestCase.TestMethod,
                        () =>
                        {
                            throw new InvalidOperationException(message);
                        });

                    await failFast.RunAsync(
                        this.MessageBus, this.ConstructorArguments, this.Aggregator, this.CancellationTokenSource);

                    continue;
                }

                summary.Aggregate(await stepRunner.RunAsync());

                if (stepFailed)
                {
                    failedStepName = stepRunner.StepDisplayName;
                }
            }

            var teardowns = stepRunners.SelectMany(runner => runner.Teardowns).ToArray();
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
                    if (!MessageBus.QueueMessage(new TestCaseCleanupFailure(TestCase, teardownAggregator.ToException())))
                    {
                        CancellationTokenSource.Cancel();
                    }
                }

                summary.Time += timer.Total;
            }

            return summary.Time;
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

        private StepRunner CreateRunner(IMessageBus messageBus, Step step, int stepNumber)
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

            var displayName = string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}.{2}] {3}",
                this.DisplayName,
                this.scenarioNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepName);

            return new StepRunner(
                stepName,
                step,
                new XunitTest(this.TestCase, displayName),
                messageBus,
                this.TestClass,
                this.ConstructorArguments,
                this.TestMethod,
                this.TestMethodArguments,
                step.SkipReason,
                this.Aggregator,
                this.CancellationTokenSource);
        }
    }
}
