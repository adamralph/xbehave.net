// <copyright file="StepInvoker.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Shims;
    using Xbehave.Sdk;
    using Xunit.Sdk;

    public class StepInvoker
    {
        private readonly string stepDisplayName;
        private readonly Step step;
        private readonly ExceptionAggregator aggregator;
        private readonly List<Action> teardowns = new List<Action>();

        public StepInvoker(string stepDisplayName, Step step, ExceptionAggregator aggregator)
        {
            Guard.AgainstNullArgument("step", step);
            Guard.AgainstNullArgument("aggregator", aggregator);

            this.stepDisplayName = stepDisplayName;
            this.step = step;
            this.aggregator = aggregator;
        }

        public string StepDisplayName
        {
            get { return this.stepDisplayName; }
        }

        public IEnumerable<Action> Teardowns
        {
            get { return this.teardowns.ToArray(); }
        }

        public async Task<decimal> RunAsync()
        {
            var timer = new ExecutionTimer();
            var oldSyncContext = SynchronizationContext.Current;

            try
            {
                var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                SetSynchronizationContext(asyncSyncContext);

                await this.aggregator.RunAsync(
                    () => timer.AggregateAsync(
                        async () =>
                        {
                            try
                            {
                                var result = this.step.Body();
                                var task = result as Task;
                                if (task != null)
                                {
                                    await task;
                                }
                            }
                            finally
                            {
                                this.teardowns.AddRange(this.step.Disposables.Select(disposable => (Action)disposable.Dispose));
                                this.teardowns.AddRange(this.step.Teardowns);
                            }

                            var ex = await asyncSyncContext.WaitForCompletionAsync();
                            if (ex != null)
                            {
                                this.aggregator.Add(ex);
                            }
                        }));
            }
            finally
            {
                SetSynchronizationContext(oldSyncContext);
            }

            return timer.Total;
        }

        [SuppressMessage(
            "Microsoft.Security",
            "CA2136:TransparencyAnnotationsShouldNotConflictFxCopRule",
            Justification = "From xunit.")]
        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
