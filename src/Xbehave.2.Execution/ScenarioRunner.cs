// <copyright file="ScenarioRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Shims;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunner : XunitTestCaseRunner
    {
        private readonly int scenarioNumber;

        public ScenarioRunner(
            MethodInfo testMethod,
            IXunitTestCase testCase,
            string displayName,
            int scenarioNumber,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                testMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
            Guard.AgainstNullArgument("testMethod", testMethod);

            this.TestMethod = testMethod;
            this.scenarioNumber = scenarioNumber;
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            if (!string.IsNullOrEmpty(this.SkipReason))
            {
                var message = new TestSkipped(new XunitTest(TestCase, DisplayName), this.SkipReason);
                if (!this.MessageBus.QueueMessage(message))
                {
                    CancellationTokenSource.Cancel();
                }

                return new RunSummary { Skipped = 1 };
            }

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
            StepRunner teardownRunner = null;
            try
            {
                var type = Reflector.GetType(
                    this.TestCase.TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name,
                    this.TestCase.TestMethod.TestClass.Class.Name);

                var obj = this.TestMethod.IsStatic ? null : Activator.CreateInstance(type, this.ConstructorArguments);
                foreach (var backgroundMethod in this.TestCase.TestMethod.Method.Type
                    .GetMethods(false)
                    .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                    .Select(method => method.ToRuntimeMethod()))
                {
                    await backgroundMethod.InvokeAsync(obj, null);
                }

                await this.TestMethod.InvokeAsync(obj, this.TestMethodArguments);

                var teardowns = new List<Action>();
                stepRunners.AddRange(CurrentScenario.ExtractSteps()
                    .Select((step, index) =>
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

                        teardowns.AddRange(step.Teardowns);

                        return new StepRunner(
                            stepName,
                            step.Body,
                            new XunitTest(this.TestCase, GetDisplayName(++index, stepName)),
                            interceptingBus,
                            this.TestClass,
                            this.ConstructorArguments,
                            this.TestMethod,
                            this.TestMethodArguments,
                            step.SkipReason,
                            this.Aggregator,
                            this.CancellationTokenSource);
                    }));

                if (teardowns.Any())
                {
                    teardownRunner = new StepRunner(
                        "(Teardown)",
                        () =>
                        {
                            teardowns.Reverse();
                            Exception exception = null;
                            foreach (var teardown in teardowns)
                            {
                                try
                                {
                                    teardown();
                                }
                                catch (Exception ex)
                                {
                                    exception = ex;
                                }
                            }

                            if (exception != null)
                            {
                                ExceptionDispatchInfo.Capture(exception).Throw();
                            }

                            return null;
                        },
                        new XunitTest(this.TestCase, this.GetDisplayName(stepRunners.Count + 1, "(Teardown)")),
                        interceptingBus,
                        this.TestClass,
                        this.ConstructorArguments,
                        this.TestMethod,
                        this.TestMethodArguments,
                        string.Empty,
                        this.Aggregator,
                        this.CancellationTokenSource);
                }
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

                return new RunSummary { Total = 1, Failed = 1 };
            }

            var summary = new RunSummary();
            string failedStepName = null;
            foreach (var stepRunner in stepRunners)
            {
                if (failedStepName != null)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to execute preceding step \"{0}\".",
                        failedStepName);

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
                    failedStepName = stepRunner.StepName;
                }
            }

            if (teardownRunner != null)
            {
                summary.Aggregate(await teardownRunner.RunAsync());
            }

            return summary;
        }

        private string GetDisplayName(int index, string stepName)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}.{2}] {3}",
                this.DisplayName,
                this.scenarioNumber.ToString("D2", CultureInfo.InvariantCulture),
                index.ToString("D2", CultureInfo.InvariantCulture),
                stepName);
        }
    }
}
