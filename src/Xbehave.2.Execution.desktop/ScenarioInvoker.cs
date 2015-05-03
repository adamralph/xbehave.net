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
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Extensions;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioInvoker : XunitTestInvoker
    {
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
            : base(
                scenario,
                messageBus,
                scenarioClass,
                constructorArguments,
                scenarioMethod,
                scenarioMethodArguments,
                beforeAfterScenarioAttributes,
                aggregator,
                cancellationTokenSource)
        {
        }

        public override async Task<decimal> InvokeTestMethodAsync(object testClassInstance)
        {
            await this.Aggregator.RunAsync(async () =>
            {
                using (ThreadStaticStepHub.CreateBackgroundSteps())
                {
                    foreach (var backgroundMethod in this.TestClass.GetMethods()
                        .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any()))
                    {
                        await this.Timer.AggregateAsync(() => backgroundMethod.InvokeAsync(testClassInstance, null));
                    }
                }
            });

            await base.InvokeTestMethodAsync(testClassInstance);

            if (!this.Aggregator.HasExceptions)
            {
                await this.InvokeStepsAsync(ThreadStaticStepHub.RemoveAll());
            }

            return this.Timer.Total;
        }

        // TODO: stop taking StepDefinitions as a param
        protected virtual async Task<decimal> InvokeStepsAsync(IList<StepDefinition> stepDefinitions)
        {
            string skipReason = null;
            var teardowns = new List<Action>();
            foreach (var item in stepDefinitions.Select((definition, index) => new { definition, index }))
            {
                item.definition.SkipReason = item.definition.SkipReason ?? skipReason;

                var stepDisplayName = GetStepDisplayName(item.definition.Text, this.TestMethodArguments);

                var childAggregator = new ExceptionAggregator();
                var disposables = (await this.InvokeStepAsync(item.definition.Body, childAggregator)).Item2;
                teardowns.AddRange(disposables.Select(disposable => (Action)disposable.Dispose)
                    .Concat(item.definition.Teardowns).ToArray());

                var exception = childAggregator.ToException();
                if (exception == null)
                {
                    this.MessageBus.QueueMessage(new TestOutput(this.Test, stepDisplayName));
                }
                else
                {
                    this.Aggregator.Add(exception);
                    var output = string.Format(
                        CultureInfo.InvariantCulture, "{0} (Failed){1}{2}", stepDisplayName, Environment.NewLine, exception);

                    this.MessageBus.QueueMessage(new TestOutput(this.Test, output));
                }
            }

            if (teardowns.Any())
            {
                teardowns.Reverse();
                var teardownTimer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();
                foreach (var teardown in teardowns)
                {
                    this.Timer.Aggregate(() => teardownTimer.Aggregate(() => teardownAggregator.Run(() => teardown())));
                }

                if (teardownAggregator.HasExceptions)
                {
                    var stepDisplayName = GetStepDisplayName("(Teardown)", this.TestMethodArguments);

                    this.MessageBus.Queue(
                        new XunitTest(this.TestCase, stepDisplayName),
                        test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException()),
                        this.CancellationTokenSource);
                }
            }

            return this.Timer.Total;
        }

        protected virtual async Task<Tuple<decimal, IDisposable[]>> InvokeStepAsync(Func<IStepContext, Task> body, ExceptionAggregator aggregator)
        {
            IDisposable[] disposables = null;
            await aggregator.RunAsync(async () =>
            {
                var stepContext = new StepContext();
                var oldSyncContext = SynchronizationContext.Current;
                try
                {
                    var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                    SetSynchronizationContext(asyncSyncContext);

                    await aggregator.RunAsync(
                        () => this.Timer.AggregateAsync(
                            async () =>
                            {
                                await body(stepContext);
                                var ex = await asyncSyncContext.WaitForCompletionAsync();
                                if (ex != null)
                                {
                                    aggregator.Add(ex);
                                }
                            }));
                }
                finally
                {
                    SetSynchronizationContext(oldSyncContext);
                }

                disposables = stepContext.Disposables.ToArray();
            });

            return Tuple.Create(this.Timer.Total, disposables ?? new IDisposable[0]);
        }

        [SuppressMessage("Microsoft.Security", "CA2136:TransparencyAnnotationsShouldNotConflictFxCopRule", Justification = "From xunit.")]
        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }

        private static string GetStepDisplayName(string stepNameFormat, IEnumerable<object> scenarioMethodArguments)
        {
            try
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    stepNameFormat ?? string.Empty,
                    (scenarioMethodArguments ?? Enumerable.Empty<object>()).Select(argument => argument ?? "null")
                        .ToArray());
            }
            catch (FormatException)
            {
                return stepNameFormat;
            }
        }
    }
}
