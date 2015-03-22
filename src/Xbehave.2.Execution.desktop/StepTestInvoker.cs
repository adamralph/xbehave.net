// <copyright file="StepTestInvoker.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Sdk;

    public class StepTestInvoker
    {
        private readonly ExecutionTimer timer = new ExecutionTimer();
        private readonly Step step;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;

        public StepTestInvoker(
            Step step, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("step", step);

            this.step = step;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        protected ExecutionTimer Timer
        {
            get { return this.timer; }
        }

        protected Step Step
        {
            get { return this.step; }
        }

        protected ExceptionAggregator Aggregator
        {
            get { return this.aggregator; }
        }

        protected CancellationTokenSource CancellationTokenSource
        {
            get { return this.cancellationTokenSource; }
        }

        public Task<Tuple<decimal, Action[]>> RunAsync()
        {
            return this.aggregator.RunAsync(async () =>
            {
                Action[] teardowns = null;
                if (!this.cancellationTokenSource.IsCancellationRequested && !this.aggregator.HasExceptions)
                {
                    teardowns = await InvokeTestMethodAsync();
                }

                return Tuple.Create(this.timer.Total, teardowns ?? new Action[0]);
            });
        }

        protected virtual async Task<Action[]> InvokeTestMethodAsync()
        {
            var oldSyncContext = SynchronizationContext.Current;
            Action[] teardowns = null;
            try
            {
                var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                SetSynchronizationContext(asyncSyncContext);

                await this.aggregator.RunAsync(
                    () => this.timer.AggregateAsync(
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
                                teardowns = this.step.Disposables.Select(disposable => (Action)disposable.Dispose)
                                    .Concat(this.step.Teardowns).ToArray();
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

            return teardowns ?? new Action[0];
        }

        [SuppressMessage("Microsoft.Security", "CA2136:TransparencyAnnotationsShouldNotConflictFxCopRule", Justification = "From xunit.")]
        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
