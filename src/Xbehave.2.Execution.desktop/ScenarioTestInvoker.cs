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

    public class ScenarioTestInvoker
    {
        private readonly ExecutionTimer timer = new ExecutionTimer();
        private readonly int scenarioNumber;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes;
        private readonly Stack<BeforeAfterTestAttribute> beforeAfterAttributesRun = new Stack<BeforeAfterTestAttribute>();

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
        {
            this.scenarioNumber = scenarioNumber;
            this.Test = test;
            this.MessageBus = messageBus;
            this.TestClass = testClass;
            this.ConstructorArguments = constructorArguments;
            this.TestMethod = testMethod;
            this.TestMethodArguments = testMethodArguments;
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

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterAttributes
        {
            get { return this.beforeAfterAttributes; }
        }

        protected ExceptionAggregator Aggregator { get; set; }

        protected CancellationTokenSource CancellationTokenSource { get; set; }

        protected ExecutionTimer Timer
        {
            get { return this.timer; }
        }

        public Task<RunSummary> RunAsync()
        {
            var summary = new RunSummary();
            return this.Aggregator.RunAsync(async () =>
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    var testClassInstance = CreateTestClass();

                    if (!CancellationTokenSource.IsCancellationRequested)
                    {
                        await BeforeTestMethodInvokedAsync();

                        if (!Aggregator.HasExceptions)
                        {
                            summary.Aggregate(await InvokeTestMethodAsync(testClassInstance));
                        }

                        await AfterTestMethodInvokedAsync();
                    }

                    var disposable = testClassInstance as IDisposable;
                    if (disposable != null)
                    {
                        Aggregator.Run(() => timer.Aggregate(disposable.Dispose));
                    }
                }

                summary.Time += this.timer.Total;
                return summary;
            });
        }

        protected virtual object CreateTestClass()
        {
            object testClass = null;

            if (!this.TestMethod.IsStatic && !this.Aggregator.HasExceptions)
            {
                this.timer.Aggregate(() => testClass = Activator.CreateInstance(this.TestClass, this.ConstructorArguments));
            }

            return testClass;
        }

        protected virtual Task BeforeTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterAttributes)
            {
                try
                {
                    this.timer.Aggregate(() => beforeAfterAttribute.Before(this.TestMethod));
                    this.beforeAfterAttributesRun.Push(beforeAfterAttribute);
                }
                catch (Exception ex)
                {
                    this.Aggregator.Add(ex);
                    break;
                }
            }

            return Task.FromResult(0);
        }

        protected virtual Task AfterTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterAttributesRun)
            {
                this.Aggregator.Run(() => this.Timer.Aggregate(() => beforeAfterAttribute.After(this.TestMethod)));
            }

            return Task.FromResult(0);
        }

        protected async virtual Task<RunSummary> InvokeTestMethodAsync(object testClassInstance)
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
                await this.InvokeBackgroundMethodsAsync(testClassInstance);
                await this.timer.AggregateAsync(() =>
                    this.TestMethod.InvokeAsync(testClassInstance, this.TestMethodArguments));

                stepTestRunners.AddRange(CurrentScenario.ExtractSteps()
                    .Select((step, index) => this.CreateStepTestRunner(interceptingBus, step, index + 1)));
            }
            catch (Exception ex)
            {
                this.MessageBus.Queue(
                    this.Test, test => new TestFailed(test, 0, null, ex.Unwrap()), this.CancellationTokenSource);

                return new RunSummary { Failed = 1, Total = 1 };
            }

            var summary = new RunSummary();
            string failedStepName = null;
            foreach (var stepTestRunner in stepTestRunners)
            {
                if (failedStepName != null)
                {
                    summary.Failed++;
                    summary.Total++;
                    var message = string.Format(
                        CultureInfo.InvariantCulture, "Failed to execute preceding step \"{0}\".", failedStepName);

                    this.MessageBus.Queue(
                        new XunitTest((IXunitTestCase)this.Test.TestCase, stepTestRunner.TestDisplayName),
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
                var teardownTimer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();

                foreach (var teardown in teardowns.Reverse())
                {
                    teardownTimer.Aggregate(() => teardownAggregator.Run(() => teardown()));
                }

                if (teardownAggregator.HasExceptions)
                {
                    summary.Failed++;
                    summary.Total++;
                    this.MessageBus.Queue(
                        new XunitTest(
                            (IXunitTestCase)this.Test.TestCase,
                            GetDisplayName(this.Test.DisplayName, this.scenarioNumber, stepTestRunners.Count + 1, "(Teardown)")),
                        test => new TestFailed(test, 0, null, teardownAggregator.ToException()),
                        this.CancellationTokenSource);
                }

                summary.Time += teardownTimer.Total;
            }

            return summary;
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

        private async Task InvokeBackgroundMethodsAsync(object testClassInstance)
        {
            CurrentScenario.AddingBackgroundSteps = true;
            try
            {
                foreach (var backgroundMethod in this.Test.TestCase.TestMethod.Method.Type
                    .GetMethods(false)
                    .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                    .Select(method => method.ToRuntimeMethod()))
                {
                    await this.timer.AggregateAsync(() => backgroundMethod.InvokeAsync(testClassInstance, null));
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
                new XunitTest(
                    (IXunitTestCase)this.Test.TestCase,
                    GetDisplayName(this.Test.DisplayName, this.scenarioNumber, stepNumber, stepName)),
                messageBus,
                this.TestClass,
                this.ConstructorArguments,
                this.TestMethod,
                this.TestMethodArguments,
                step.SkipReason,
                this.beforeAfterAttributes,
                new ExceptionAggregator(this.Aggregator),
                this.CancellationTokenSource);
        }
    }
}
