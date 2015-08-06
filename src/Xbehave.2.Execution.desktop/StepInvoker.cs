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
        private readonly IStep step;
        private readonly Func<IStepContext, Task> body;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ExecutionTimer timer = new ExecutionTimer();

        public StepInvoker(
            IStep step,
            Func<IStepContext, Task> body,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("aggregator", aggregator);
            Guard.AgainstNullArgument("cancellationTokenSource", cancellationTokenSource);

            this.step = step;
            this.body = body;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public async Task<Tuple<decimal, IDisposable[]>> RunAsync()
        {
            IDisposable[] disposables = null;
            await this.aggregator.RunAsync(async () =>
            {
                if (!this.cancellationTokenSource.IsCancellationRequested && !this.aggregator.HasExceptions)
                {
                    disposables = await this.InvokeBodyAsync();
                }
            });

            return Tuple.Create(this.timer.Total, disposables ?? new IDisposable[0]);
        }

        [SuppressMessage("Microsoft.Security", "CA2136:TransparencyAnnotationsShouldNotConflictFxCopRule", Justification = "From xunit.")]
        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }

        private async Task<IDisposable[]> InvokeBodyAsync()
        {
            var stepContext = new StepContext(this.step);
            if (this.body != null)
            {
                var oldSyncContext = SynchronizationContext.Current;
                try
                {
                    var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                    SetSynchronizationContext(asyncSyncContext);

                    await this.aggregator.RunAsync(
                        () => this.timer.AggregateAsync(
                            async () =>
                            {
                                await this.body(stepContext);
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
            }

            return stepContext.Disposables.ToArray();
        }
    }
}
