// <copyright file="ScenarioTestGroupInvoker.cs" company="xBehave.net contributors">
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

    public class ScenarioTestGroupInvoker
    {
        private readonly ExecutionTimer timer = new ExecutionTimer();
        private readonly int scenarioNumber;
        private readonly IScenarioTestGroup testGroup;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes;
        private readonly Stack<BeforeAfterTestAttribute> beforeAfterTestGroupAttributesRun =
            new Stack<BeforeAfterTestAttribute>();

        public ScenarioTestGroupInvoker(
            int scenarioNumber,
            IScenarioTestGroup testGroup,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
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

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterTestGroupAttributes
        {
            get { return this.beforeAfterTestGroupAttributes; }
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

                        if (!this.CancellationTokenSource.IsCancellationRequested && !this.Aggregator.HasExceptions)
                        {
                            summary.Aggregate(await InvokeTestMethodAsync(testClassInstance));
                        }

                        await AfterTestMethodInvokedAsync();
                    }

                    var disposable = testClassInstance as IDisposable;
                    if (disposable != null)
                    {
                        timer.Aggregate(() => Aggregator.Run(disposable.Dispose));
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
            foreach (var beforeAfterAttribute in this.beforeAfterTestGroupAttributes)
            {
                try
                {
                    this.timer.Aggregate(() => beforeAfterAttribute.Before(this.TestMethod));
                    this.beforeAfterTestGroupAttributesRun.Push(beforeAfterAttribute);
                }
                catch (Exception ex)
                {
                    this.Aggregator.Add(ex);
                    break;
                }

                if (CancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }

            return Task.FromResult(0);
        }

        protected virtual Task AfterTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterTestGroupAttributesRun)
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

            var stepDiscoveryTimer = new ExecutionTimer();
            var stepTestRunners = new Dictionary<string, StepTestRunner>();
            try
            {
                await this.InvokeBackgroundMethodsAsync(testClassInstance, stepDiscoveryTimer);
                await stepDiscoveryTimer.AggregateAsync(() =>
                    this.TestMethod.InvokeAsync(testClassInstance, this.TestMethodArguments));

                foreach (var pair in CurrentScenario.ExtractSteps()
                    .Select((step, index) => this.CreateStepTestRunner(interceptingBus, step, index + 1)))
                {
                    stepTestRunners.Add(pair.Key, pair.Value);
                }
            }
            catch (Exception ex)
            {
                this.MessageBus.Queue(
                    new XunitTest(this.testGroup.TestCase, this.testGroup.DisplayName),
                    test => new TestFailed(test, stepDiscoveryTimer.Total, null, ex.Unwrap()),
                    this.CancellationTokenSource);

                return new RunSummary { Failed = 1, Total = 1, Time = stepDiscoveryTimer.Total };
            }

            if (!stepTestRunners.Any())
            {
                this.MessageBus.Queue(
                    new XunitTest(this.testGroup.TestCase, this.testGroup.DisplayName),
                    test => new TestPassed(test, stepDiscoveryTimer.Total, null),
                    this.CancellationTokenSource);

                return new RunSummary { Total = 1, Time = stepDiscoveryTimer.Total };
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
                        new XunitTest(this.testGroup.TestCase, stepTestRunner.Key),
                        test => new TestFailed(test, 0, string.Empty, new InvalidOperationException(message)),
                        this.CancellationTokenSource);

                    continue;
                }

                summary.Aggregate(await stepTestRunner.Value.RunAsync());

                if (stepFailed)
                {
                    failedStepName = stepTestRunner.Value.StepDisplayName;
                }
            }

            var teardowns = stepTestRunners.SelectMany(runner => runner.Value.Teardowns).ToArray();
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
                    summary.Time += teardownTimer.Total;
                    var stepTestDisplayName = GetStepTestDisplayName(
                        this.TestGroup.DisplayName, this.scenarioNumber, stepTestRunners.Count + 1, "(Teardown)");

                    this.MessageBus.Queue(
                        new XunitTest(this.testGroup.TestCase, stepTestDisplayName),
                        test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException()),
                        this.CancellationTokenSource);
                }
            }

            return summary;
        }

        private async Task InvokeBackgroundMethodsAsync(object testClassInstance, ExecutionTimer stepDiscoveryTimer)
        {
            CurrentScenario.AddingBackgroundSteps = true;
            try
            {
                foreach (var backgroundMethod in this.testGroup.TestCase.TestMethod.Method.Type
                    .GetMethods(false)
                    .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                    .Select(method => method.ToRuntimeMethod()))
                {
                    await stepDiscoveryTimer.AggregateAsync(() => backgroundMethod.InvokeAsync(testClassInstance, null));
                }
            }
            finally
            {
                CurrentScenario.AddingBackgroundSteps = false;
            }
        }

        private KeyValuePair<string, StepTestRunner> CreateStepTestRunner(
            IMessageBus messageBus, Step step, int stepNumber)
        {
            string stepDisplayName;
            try
            {
                stepDisplayName = string.Format(
                    CultureInfo.InvariantCulture,
                    step.Name,
                    this.TestMethodArguments.Select(argument => argument ?? "null").ToArray());
            }
            catch (FormatException)
            {
                stepDisplayName = step.Name;
            }

            var displayName = GetStepTestDisplayName(this.TestGroup.DisplayName, this.scenarioNumber, stepNumber, stepDisplayName);

            return new KeyValuePair<string, StepTestRunner>(
                displayName,
                new StepTestRunner(
                    stepDisplayName,
                    step,
                    new XunitTest(this.TestGroup.TestCase, displayName),
                    messageBus,
                    this.TestClass,
                    this.ConstructorArguments,
                    this.TestMethod,
                    this.TestMethodArguments,
                    step.SkipReason,
                    this.beforeAfterTestGroupAttributes,
                    new ExceptionAggregator(this.Aggregator),
                    this.CancellationTokenSource));
        }

        private static string GetStepTestDisplayName(string scenarioName, int scenarioNumber, int stepNumber, string stepName)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}.{2}] {3}",
                scenarioName,
                scenarioNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepName);
        }
    }
}
