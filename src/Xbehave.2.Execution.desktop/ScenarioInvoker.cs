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
    using Xbehave.Abstractions;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioInvoker
    {
        private readonly ITest scenario;
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
            ITest scenario,
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

        protected ITest Scenario
        {
            get { return this.scenario; }
        }

        protected IMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        protected Type ScenarioClass
        {
            get { return this.scenarioClass; }
        }

        protected IReadOnlyList<object> ConstructorArguments
        {
            get { return this.constructorArguments; }
        }

        protected MethodInfo ScenarioMethod
        {
            get { return this.scenarioMethod; }
        }

        protected IReadOnlyList<object> ScenarioMethodArguments
        {
            get { return this.scenarioMethodArguments; }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterScenarioAttributes
        {
            get { return this.beforeAfterScenarioAttributes; }
        }

        protected ExceptionAggregator Aggregator
        {
            get { return this.aggregator; }
        }

        protected CancellationTokenSource CancellationTokenSource
        {
            get { return this.cancellationTokenSource; }
        }

        protected ExecutionTimer Timer
        {
            get { return this.timer; }
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

        protected virtual object CreateScenarioClass()
        {
            object testClass = null;

            if (!this.scenarioMethod.IsStatic && !this.aggregator.HasExceptions)
            {
                this.timer.Aggregate(() => testClass = Activator.CreateInstance(this.scenarioClass, this.constructorArguments));
            }

            return testClass;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are collected in the aggregator.")]
        protected virtual Task BeforeScenarioMethodInvokedAsync()
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

        protected virtual Task AfterScenarioMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterScenarioAttributesRun)
            {
                this.aggregator.Run(() => this.timer.Aggregate(() => beforeAfterAttribute.After(this.scenarioMethod)));
            }

            return Task.FromResult(0);
        }

        protected async virtual Task<RunSummary> InvokeScenarioMethodAsync(object scenarioClassInstance)
        {
            await this.aggregator.RunAsync(async () =>
            {
                using (ThreadStaticStepHub.CreateBackgroundSteps())
                {
                    foreach (var backgroundMethod in this.scenario.TestCase.TestMethod.Method.Type
                        .GetMethods(false)
                        .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                        .Select(method => method.ToRuntimeMethod()))
                    {
                        await this.timer.AggregateAsync(() =>
                            backgroundMethod.InvokeAsync(scenarioClassInstance, null));
                    }
                }

                await this.timer.AggregateAsync(() =>
                    this.scenarioMethod.InvokeAsync(scenarioClassInstance, this.scenarioMethodArguments));
            });

            var runSummary = new RunSummary();
            if (!this.aggregator.HasExceptions)
            {
                runSummary.Aggregate(await this.InvokeStepsAsync(ThreadStaticStepHub.RemoveAll()));
            }

            return runSummary;
        }

        // TODO: stop taking StepDefinitions as a param
        protected async virtual Task<RunSummary> InvokeStepsAsync(IList<StepDefinition> stepDefinitions)
        {
            var summary = new RunSummary();
            string skipReason = null;
            var teardowns = new List<Action>();
            foreach (var item in stepDefinitions.Select((definition, index) => new { definition, index }))
            {
                item.definition.SkipReason = item.definition.SkipReason ?? skipReason;

                var stepDisplayName = GetStepDisplayName(
                    this.scenario.DisplayName, item.index + 1, item.definition.Text, this.scenarioMethodArguments);

                var step = new Step(this.scenario, stepDisplayName);

                var interceptingBus = new DelegatingMessageBus(
                    this.messageBus,
                    message =>
                    {
                        if (message is ITestFailed && !item.definition.ContinueOnFailure)
                        {
                            skipReason = string.Format(
                                CultureInfo.InvariantCulture,
                                "Failed to execute preceding step: {0}",
                                step.DisplayName);
                        }
                    });

                var stepRunner = new StepRunner(
                    step,
                    item.definition.Body,
                    interceptingBus,
                    this.scenarioClass,
                    this.constructorArguments,
                    this.scenarioMethod,
                    this.scenarioMethodArguments,
                    item.definition.SkipReason,
                    new ExceptionAggregator(this.aggregator),
                    this.cancellationTokenSource);

                summary.Aggregate(await stepRunner.RunAsync());
                teardowns.AddRange(stepRunner.Disposables.Select(disposable => (Action)disposable.Dispose)
                    .Concat(item.definition.Teardowns).ToArray());
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
                        this.scenario.DisplayName, stepDefinitions.Count + 1, "(Teardown)", this.scenarioMethodArguments);

                    this.messageBus.Queue(
                        new Step(this.scenario, stepDisplayName),
                        test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException()),
                        this.cancellationTokenSource);
                }
            }

            return summary;
        }

        private static string GetStepDisplayName(
            string scenarioDisplayName, int stepNumber, string stepNameFormat, IEnumerable<object> scenarioMethodArguments)
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
                "{0} [{1}] {2}",
                scenarioDisplayName,
                stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepName);
        }
    }
}
