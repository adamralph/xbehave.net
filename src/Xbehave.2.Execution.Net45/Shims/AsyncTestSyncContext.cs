// <copyright file="AsyncTestSyncContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Shims
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
#if WPA81
    using Windows.System.Threading;
#endif

    internal class AsyncTestSyncContext : SynchronizationContext
    {
        private readonly AsyncManualResetEvent @event = new AsyncManualResetEvent(true);
        private readonly SynchronizationContext innerContext;
        private Exception exception;
        private int operationCount;

        public AsyncTestSyncContext(SynchronizationContext innerContext)
        {
            this.innerContext = innerContext;
        }

        public override void OperationCompleted()
        {
            var result = Interlocked.Decrement(ref this.operationCount);
            if (result == 0)
            {
                @event.Set();
            }
        }

        public override void OperationStarted()
        {
            Interlocked.Increment(ref this.operationCount);
            @event.Reset();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "From xunit.")]
#if WPA81
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "unused",
            Justification = "Avoids a seemingly unsuppressable compiler warning regarding lack of 'await'.")]
#endif
        public override void Post(SendOrPostCallback d, object state)
        {
            // The call to Post() may be the state machine signaling that an exception is
            // about to be thrown, so we make sure the operation count gets incremented
            // before the Task.Run, and then decrement the count when the operation is done.
            this.OperationStarted();

            try
            {
                if (this.innerContext == null)
                {
#if WPA81
                    var unused = ThreadPool.RunAsync(
#else
                    ThreadPool.QueueUserWorkItem(
#endif
_ =>
{
    try
    {
        Send(d, state);
    }
    finally
    {
        OperationCompleted();
    }
#if WPA81
},
                        WorkItemPriority.Normal,
                        WorkItemOptions.TimeSliced);
#else
                        });
#endif
                }
                else
                {
                    this.innerContext.Post(
                        _ =>
                        {
                            try
                            {
                                Send(d, _);
                            }
                            finally
                            {
                                OperationCompleted();
                            }
                        },
                        state);
                }
            }
            catch
            {
            }
        }

        [SuppressMessage(
            "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is recorded.")]
        public override void Send(SendOrPostCallback d, object state)
        {
            Guard.AgainstNullArgument("d", d);

            try
            {
                if (this.innerContext != null)
                {
                    this.innerContext.Send(d, state);
                }
                else
                {
                    d(state);
                }
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
        }

        public async Task<Exception> WaitForCompletionAsync()
        {
            await @event.WaitAsync();
            return this.exception;
        }
    }
}