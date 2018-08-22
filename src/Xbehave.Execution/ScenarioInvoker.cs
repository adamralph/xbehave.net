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
            Guard.AgainstNullArgument(nameof(scenario), scenario);
            Guard.AgainstNullArgument(nameof(messageBus), messageBus);
            Guard.AgainstNullArgument(nameof(scenarioClass), scenarioClass);
            Guard.AgainstNullArgument(nameof(scenarioMethod), scenarioMethod);
            Guard.AgainstNullArgument(nameof(beforeAfterScenarioAttributes), beforeAfterScenarioAttributes);
            Guard.AgainstNullArgument(nameof(aggregator), aggregator);
            Guard.AgainstNullArgument(nameof(cancellationTokenSource), cancellationTokenSource);

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

                    if (testClassInstance is IDisposable disposable)
                    {
                        this.timer.Aggregate(() => this.aggregator.Run(disposable.Dispose));
                    }
                }

                summary.Time += this.timer.Total;
            });

            return summary;
        }

        private static string GetStepDisplayName(
            string scenarioDisplayName, int stepNumber, string stepDisplayText) =>
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} [{1}] {2}",
                    scenarioDisplayName,
                    stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                    stepDisplayText);

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
                    foreach (var backgroundMethod in this.scenario.TestCase.TestMethod.TestClass.Class
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
            var scenarioTypeInfo = this.scenarioClass.GetTypeInfo();
            var filters = scenarioTypeInfo.Assembly.GetCustomAttributes(typeof(Attribute))
                .Concat(scenarioTypeInfo.GetCustomAttributes(typeof(Attribute)))
                .Concat(this.scenarioMethod.GetCustomAttributes(typeof(Attribute)))
                .OfType<IFilter<IStepDefinition>>();

            var summary = new RunSummary();
            string skipReason = null;
            var scenarioTeardowns = new List<Tuple<StepContext, Func<IStepContext, Task>>>();
            var stepNumber = 0;
            foreach (var stepDefinition in filters.Aggregate(
                backGroundStepDefinitions.Concat(scenarioStepDefinitions),
                (current, filter) => filter.Filter(current)))
            {
                stepDefinition.SkipReason = stepDefinition.SkipReason ?? skipReason;

                var stepDisplayName = GetStepDisplayName(
                    this.scenario.DisplayName,
                    ++stepNumber,
                    stepDefinition.DisplayTextFunc?.Invoke(stepDefinition.Text, stepNumber <= backGroundStepDefinitions.Count));

                var step = new Step(this.scenario, stepDisplayName);

                var interceptingBus = new DelegatingMessageBus(
                    this.messageBus,
                    message =>
                    {
                        if (message is ITestFailed && stepDefinition.FailureBehavior == RemainingSteps.Skip)
                        {
                            skipReason = $"Failed to execute preceding step: {step.DisplayName}";
                        }
                    });

                var stepContext = new StepContext(step);

                var stepRunner = new StepRunner(
                    stepContext,
                    stepDefinition.Body,
                    step,
                    interceptingBus,
                    this.scenarioClass,
                    this.constructorArguments,
                    this.scenarioMethod,
                    this.scenarioMethodArguments,
                    stepDefinition.SkipReason,
                    new BeforeAfterTestAttribute[0],
                    new ExceptionAggregator(this.aggregator),
                    this.cancellationTokenSource);

                summary.Aggregate(await stepRunner.RunAsync());

                var stepTeardowns = stepContext.Disposables
                    .Select(disposable =>
                    {
                        if (disposable == null)
                        {
                            return null;
                        }

                        Func<IStepContext, Task> task = context =>
                        {
                            disposable.Dispose();
                            return Task.FromResult(0);
                        };

                        return task;
                    })
                    .Concat(stepDefinition.Teardowns)
                    .Where(teardown => teardown != null)
                    .Select(teardown => Tuple.Create(stepContext, teardown));

                scenarioTeardowns.AddRange(stepTeardowns);
            }

            if (scenarioTeardowns.Any())
            {
                scenarioTeardowns.Reverse();
                var teardownTimer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();
                foreach (var teardown in scenarioTeardowns)
                {
                    await Invoker.Invoke(() => teardown.Item2(teardown.Item1), teardownAggregator, teardownTimer);
                }

                summary.Time += teardownTimer.Total;

                if (teardownAggregator.HasExceptions)
                {
                    summary.Failed++;
                    summary.Total++;

                    var stepDisplayName = GetStepDisplayName(this.scenario.DisplayName, ++stepNumber, "(Teardown)");

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
