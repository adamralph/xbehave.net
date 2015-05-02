// <copyright file="StepInvoker.cs" company="xBehave.net contributors">
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

    public class StepInvoker
    {
        private readonly Func<IStepContext, Task> step;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ExecutionTimer timer = new ExecutionTimer();

        public StepInvoker(
            Func<IStepContext, Task> step, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("step", step);
            Guard.AgainstNullArgument("aggregator", aggregator);
            Guard.AgainstNullArgument("cancellationTokenSource", cancellationTokenSource);

            this.step = step;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        protected Func<IStepContext, object> Step
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

        protected ExecutionTimer Timer
        {
            get { return this.timer; }
        }

        public async Task<Tuple<decimal, IDisposable[]>> RunAsync()
        {
            IDisposable[] teardowns = null;
            await this.aggregator.RunAsync(async () =>
            {
                if (!this.cancellationTokenSource.IsCancellationRequested && !this.aggregator.HasExceptions)
                {
                    teardowns = await InvokeTestMethodAsync();
                }
            });

            return Tuple.Create(this.timer.Total, teardowns ?? new IDisposable[0]);
        }

        protected virtual async Task<IDisposable[]> InvokeTestMethodAsync()
        {
            var context = new StepContext();
            var oldSyncContext = SynchronizationContext.Current;
            try
            {
                var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                SetSynchronizationContext(asyncSyncContext);

                await this.aggregator.RunAsync(
                    () => this.timer.AggregateAsync(
                        async () =>
                        {
                            await this.step(context);
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

            return context.Disposables.ToArray();
        }

        [SuppressMessage("Microsoft.Security", "CA2136:TransparencyAnnotationsShouldNotConflictFxCopRule", Justification = "From xunit.")]
        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
