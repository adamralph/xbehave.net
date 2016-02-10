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

        public async Task<Tuple<decimal, IDisposable[], ExecutionContext>> RunAsync(ExecutionContext executionContext)
        {
            Tuple<IDisposable[], ExecutionContext> bodyResult = null;
            await this.aggregator.RunAsync(async () =>
            {
                if (!this.cancellationTokenSource.IsCancellationRequested && !this.aggregator.HasExceptions)
                {
                    bodyResult = await this.InvokeBodyAsync(executionContext);
                }
            });

            return Tuple.Create(this.timer.Total, bodyResult?.Item1 ?? new IDisposable[0], bodyResult?.Item2 ?? executionContext);
        }

        [SuppressMessage("Microsoft.Security", "CA2136:TransparencyAnnotationsShouldNotConflictFxCopRule", Justification = "From xunit.")]
        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }

        private async Task<Tuple<IDisposable[], ExecutionContext>> InvokeBodyAsync(ExecutionContext executionContext)
        {
            var stepContext = new StepContext(this.step);
            if (this.body != null)
            {
                var oldSyncContext = SynchronizationContext.Current;
                try
                {
                    var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                    var executionContextSyncContext =
                        new ExecutionContextSyncContext(executionContext, asyncSyncContext);
                    SetSynchronizationContext(executionContextSyncContext);

                    await this.aggregator.RunAsync(
                        () => this.timer.AggregateAsync(
                            async () =>
                            {
                                await this.body(stepContext);
                                var ex = await asyncSyncContext.WaitForCompletionAsync();
                                executionContext = ExecutionContext.Capture();
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

            return Tuple.Create(stepContext.Disposables.ToArray(), executionContext);
        }
    }
}
