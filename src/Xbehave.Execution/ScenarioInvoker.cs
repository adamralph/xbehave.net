using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xbehave.Execution.Extensions;
using Xbehave.Sdk;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xbehave.Execution
{
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
            this.scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
            this.messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            this.scenarioClass = scenarioClass ?? throw new ArgumentNullException(nameof(scenarioClass));
            this.constructorArguments = constructorArguments;
            this.scenarioMethod = scenarioMethod ?? throw new ArgumentNullException(nameof(scenarioMethod));
            this.scenarioMethodArguments = scenarioMethodArguments;
            this.beforeAfterScenarioAttributes = beforeAfterScenarioAttributes ?? throw new ArgumentNullException(nameof(beforeAfterScenarioAttributes));
            this.aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            this.cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
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

        private Task BeforeScenarioMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterScenarioAttributes)
            {
                try
                {
                    this.timer.Aggregate(() => beforeAfterAttribute.Before(this.scenarioMethod));
                    this.beforeAfterScenarioAttributesRun.Push(beforeAfterAttribute);
                }
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
#pragma warning restore IDE0079 // Remove unnecessary suppression
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
                using (CurrentThread.EnterStepDefinitionContext())
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

                using (CurrentThread.EnterStepDefinitionContext())
                {
                    await this.timer.AggregateAsync(() =>
                        this.scenarioMethod.InvokeAsync(scenarioClassInstance, this.scenarioMethodArguments));

                    scenarioStepDefinitions.AddRange(CurrentThread.StepDefinitions);
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

                var step = new StepTest(this.scenario, stepDisplayName);

                using (var interceptingBus = new DelegatingMessageBus(
                    this.messageBus,
                    message =>
                    {
                        if (message is ITestFailed && stepDefinition.FailureBehavior == RemainingSteps.Skip)
                        {
                            skipReason = $"Failed to execute preceding step: {step.DisplayName}";
                        }
                    }))
                {
                    var stepContext = new StepContext(step);

                    var stepRunner = new StepTestRunner(
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
                        .Where(disposable => disposable != null)
                        .Select((Func<IDisposable, Func<IStepContext, Task>>)(disposable =>
                            context =>
                            {
                                disposable.Dispose();
                                return Task.FromResult(0);
                            }))
                        .Concat(stepDefinition.Teardowns)
                        .Where(teardown => teardown != null)
                        .Select(teardown => Tuple.Create(stepContext, teardown));

                    scenarioTeardowns.AddRange(stepTeardowns);
                }
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
                        new StepTest(this.scenario, stepDisplayName),
                        test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException()),
                        this.cancellationTokenSource);
                }
            }

            return summary;
        }
    }
}
