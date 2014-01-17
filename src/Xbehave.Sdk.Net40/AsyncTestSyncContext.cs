// <copyright file="AsyncTestSyncContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading;

    /// <summary>
    /// Implementation from xUnit 2.0.
    /// </summary>
    internal class AsyncTestSyncContext : SynchronizationContext, IDisposable
    {
        private readonly ManualResetEvent @event = new ManualResetEvent(initialState: true);
        private Exception exception;
        private int operationCount;

        public void Dispose()
        {
            ((IDisposable)this.@event).Dispose();
        }

        public override void OperationCompleted()
        {
            var result = Interlocked.Decrement(ref this.operationCount);
            if (result == 0) 
            { 
                this.@event.Set();
            }
        }

        public override void OperationStarted()
        {
            Interlocked.Increment(ref this.operationCount);
            this.@event.Reset();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            // The call to Post() may be the state machine signaling that an exception is
            // about to be thrown, so we make sure the operation count gets incremented
            // before the QUWI, and then decrement the count when the operation is done.
            this.OperationStarted();

            ThreadPool.QueueUserWorkItem(s =>
            {
                try
                {
                    this.Send(d, state);
                }
                finally
                {
                    this.OperationCompleted();
                }
            });
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            try
            {
                d(state);
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
        }

        public Exception WaitForCompletion()
        {
            this.@event.WaitOne();
            return this.exception;
        }
    }
}