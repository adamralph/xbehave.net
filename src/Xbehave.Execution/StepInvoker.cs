using System;
using System.Threading;
using System.Threading.Tasks;
using Xbehave.Sdk;
using Xunit.Sdk;

namespace Xbehave.Execution
{
    public class StepInvoker
    {
        private readonly IStepContext stepContext;
        private readonly Func<IStepContext, Task> body;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ExecutionTimer timer = new ExecutionTimer();

        public StepInvoker(
            IStepContext stepContext,
            Func<IStepContext, Task> body,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            this.stepContext = stepContext;
            this.body = body;
            this.aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            this.cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        public async Task<decimal> RunAsync()
        {
            if (this.body != null)
            {
                await this.aggregator.RunAsync(async () =>
                {
                    if (!this.cancellationTokenSource.IsCancellationRequested && !this.aggregator.HasExceptions)
                    {
                        await Invoker.Invoke(() => this.body(this.stepContext), this.aggregator, this.timer);
                    }
                });
            }

            return this.timer.Total;
        }
    }
}
