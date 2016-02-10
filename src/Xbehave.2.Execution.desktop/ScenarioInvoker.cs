// <copyright file="ScenarioInvoker.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Extensions;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioInvoker
    {
        private readonly IScenario scenario;
        private readonly IMessageBus messageBus;
        private readonly Type scenarioClass;
        private readonly object[] constructorArguments;
        private readonly MethodInfo scenarioMethod;
        private readonly object[] scenarioMethodArguments;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ExecutionTimer timer = new ExecutionTimer();
        private readonly Stack<BeforeAfterTestAttribute> beforeAfterScenarioAttributesRun =
            new Stack<BeforeAfterTestAttribute>();

        public ScenarioInvoker(
            IScenario scenario,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            MethodInfo scenarioMethod,
            object[] scenarioMethodArguments,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("scenario", scenario);
            Guard.AgainstNullArgument("messageBus", messageBus);
            Guard.AgainstNullArgument("scenarioClass", scenarioClass);
            Guard.AgainstNullArgument("scenarioMethod", scenarioMethod);
            Guard.AgainstNullArgument("beforeAfterScenarioAttributes", beforeAfterScenarioAttributes);
            Guard.AgainstNullArgument("aggregator", aggregator);
            Guard.AgainstNullArgument("cancellationTokenSource", cancellationTokenSource);

            this.scenario = scenario;
            this.messageBus = messageBus;
            this.scenarioClass = scenarioClass;
            this.constructorArguments = constructorArguments;
            this.scenarioMethod = scenarioMethod;
            this.scenarioMethodArguments = scenarioMethodArguments;
            this.beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public async Task<RunSummary> RunAsync()
        {
            var summary = new RunSummary();
            await this.aggregator.RunAsync(async () =>
            {
                if (!this.cancellationTokenSource.IsCancellationRequested)
                {
                    var testClassInstance = this.CreateScenarioClass();

                    if (!this.cancellationTokenSource.IsCancellationRequested)
                    {
                        await this.BeforeScenarioMethodInvokedAsync();

                        if (!this.cancellationTokenSource.IsCancellationRequested && !this.aggregator.HasExceptions)
                        {
                            summary.Aggregate(await this.InvokeScenarioMethodAsync(testClassInstance));
                        }

                        await this.AfterScenarioMethodInvokedAsync();
                    }

                    var disposable = testClassInstance as IDisposable;
                    if (disposable != null)
                    {
                        this.timer.Aggregate(() => this.aggregator.Run(disposable.Dispose));
                    }
                }

                summary.Time += this.timer.Total;
            });

            return summary;
        }

        private static string GetStepDisplayName(
            string scenarioDisplayName,
            int stepNumber,
            bool isBackgroundStep,
            string stepNameFormat,
            IEnumerable<object> scenarioMethodArguments)
        {
            string stepName;
            try
            {
                stepName = string.Format(
                    CultureInfo.InvariantCulture,
                    stepNameFormat ?? string.Empty,
                    (scenarioMethodArguments ?? Enumerable.Empty<object>()).Select(argument => argument ?? "null").ToArray());
            }
            catch (FormatException)
            {
                stepName = stepNameFormat;
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}] {2}{3}",
                scenarioDisplayName,
                stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                isBackgroundStep ? "(Background) " : null,
                stepName);
        }

        private object CreateScenarioClass()
        {
            object testClass = null;

            if (!this.scenarioMethod.IsStatic && !this.aggregator.HasExceptions)
            {
                this.timer.Aggregate(() => testClass = Activator.CreateInstance(this.scenarioClass, this.constructorArguments));
            }

            return testClass;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are collected in the aggregator.")]
        private Task BeforeScenarioMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterScenarioAttributes)
            {
                try
                {
                    this.timer.Aggregate(() => beforeAfterAttribute.Before(this.scenarioMethod));
                    this.beforeAfterScenarioAttributesRun.Push(beforeAfterAttribute);
                }
                catch (Exception ex)
                {
                    this.aggregator.Add(ex);
                    break;
                }

                if (this.cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }

            return Task.FromResult(0);
        }

        private Task AfterScenarioMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterScenarioAttributesRun)
            {
                this.aggregator.Run(() => this.timer.Aggregate(() => beforeAfterAttribute.After(this.scenarioMethod)));
            }

            return Task.FromResult(0);
        }

        private async Task<RunSummary> InvokeScenarioMethodAsync(object scenarioClassInstance)
        {
            var backgroundStepDefinitions = new List<IStepDefinition>();
            var scenarioStepDefinitions = new List<IStepDefinition>();
            await this.aggregator.RunAsync(async () =>
            {
                try
                {
                    foreach (var backgroundMethod in this.scenario.TestCase.TestMethod.Method.Type
                        .GetMethods(false)
                        .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                        .Select(method => method.ToRuntimeMethod()))
                    {
                        await this.timer.AggregateAsync(() =>
                            backgroundMethod.InvokeAsync(scenarioClassInstance, null));
                    }

                    backgroundStepDefinitions.AddRange(CurrentThread.StepDefinitions);
                }
                finally
                {
                    CurrentThread.StepDefinitions.Clear();
                }

                try
                {
                    await this.timer.AggregateAsync(() =>
                        this.scenarioMethod.InvokeAsync(scenarioClassInstance, this.scenarioMethodArguments));

                    scenarioStepDefinitions.AddRange(CurrentThread.StepDefinitions);
                }
                finally
                {
                    CurrentThread.StepDefinitions.Clear();
                }
            });

            var runSummary = new RunSummary { Time = this.timer.Total };
            if (!this.aggregator.HasExceptions)
            {
                runSummary.Aggregate(await this.InvokeStepsAsync(backgroundStepDefinitions, scenarioStepDefinitions));
            }

            return runSummary;
        }

        private async Task<RunSummary> InvokeStepsAsync(
            ICollection<IStepDefinition> backGroundStepDefinitions, ICollection<IStepDefinition> scenarioStepDefinitions)
        {
            var filters = this.scenarioClass.Assembly.GetCustomAttributes(typeof(Attribute))
                .Concat(this.scenarioClass.GetCustomAttributes(typeof(Attribute)))
                .Concat(this.scenarioMethod.GetCustomAttributes(typeof(Attribute)))
                .OfType<IFilter<IStepDefinition>>();

            var stepDefinitions = filters
                .Aggregate(
                    backGroundStepDefinitions.Concat(scenarioStepDefinitions),
                    (current, filter) => filter.Filter(current))
                .ToArray();

            var summary = new RunSummary();
            string skipReason = null;
            var teardowns = new List<Action>();
            var stepNumber = 0;
            var executionContext = ExecutionContext.Capture();
            foreach (var stepDefinition in stepDefinitions)
            {
                stepDefinition.SkipReason = stepDefinition.SkipReason ?? skipReason;

                var stepDisplayName = GetStepDisplayName(
                    this.scenario.DisplayName,
                    ++stepNumber,
                    stepNumber <= backGroundStepDefinitions.Count,
                    stepDefinition.Text,
                    this.scenarioMethodArguments);

                var step = new Step(this.scenario, stepDisplayName);

                var interceptingBus = new DelegatingMessageBus(
                    this.messageBus,
                    message =>
                    {
                        if (message is ITestFailed && stepDefinition.FailureBehavior == RemainingSteps.Skip)
                        {
                            skipReason = string.Format(
                                CultureInfo.InvariantCulture,
                                "Failed to execute preceding step: {0}",
                                step.DisplayName);
                        }
                    });

                var stepRunner = new StepRunner(
                    step,
                    stepDefinition.Body,
                    executionContext.CreateCopy(),
                    interceptingBus,
                    this.scenarioClass,
                    this.constructorArguments,
                    this.scenarioMethod,
                    this.scenarioMethodArguments,
                    stepDefinition.SkipReason,
                    new ExceptionAggregator(this.aggregator),
                    this.cancellationTokenSource);

                summary.Aggregate(await stepRunner.RunAsync());
                executionContext = stepRunner.ExecutionContext;
                teardowns.AddRange(stepRunner.Disposables.Select(disposable => (Action)disposable.Dispose)
                    .Concat(stepDefinition.Teardowns.Where(teardown => teardown != null)).ToArray());
            }

            if (teardowns.Any())
            {
                teardowns.Reverse();
                var teardownTimer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();
                foreach (var teardown in teardowns)
                {
                    teardownTimer.Aggregate(() => teardownAggregator.Run(() => teardown()));
                }

                summary.Time += teardownTimer.Total;

                if (teardownAggregator.HasExceptions)
                {
                    summary.Failed++;
                    summary.Total++;

                    var stepDisplayName = GetStepDisplayName(
                        this.scenario.DisplayName,
                        ++stepNumber,
                        false,
                        "(Teardown)",
                        this.scenarioMethodArguments);

                    this.messageBus.Queue(
                        new Step(this.scenario, stepDisplayName),
                        test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException()),
                        this.cancellationTokenSource);
                }
            }

            return summary;
        }
    }
}
