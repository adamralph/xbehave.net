// <copyright file="AsyncTestSyncContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Shims
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try
                        {
                            Send(d, state);
                        }
                        finally
                        {
                            OperationCompleted();
                        }
                    });
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

        public override void Send(SendOrPostCallback d, object state)
        {
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