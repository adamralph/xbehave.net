// <copyright file="SyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Xunit.Sdk;

    public class SyncStep : Step
    {
        private readonly Action body;

        public SyncStep(string name, Action body, object stepType)
            : base(name, stepType)
        {
            Guard.AgainstNullArgument("body", body);

            this.body = body;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Capturing an re-throwing an exception from another thread.")]
        public override void Execute()
        {
            try
            {
                Exception exception = null;
                var @event = new ManualResetEvent(false);

                ThreadPool.QueueUserWorkItem(o =>
                {
                    var oldSyncContext = SynchronizationContext.Current;
                    using (var syncContext = new AsyncTestSyncContext())
                    {
                        SynchronizationContext.SetSynchronizationContext(syncContext);

                        try
                        {
                            this.body.Invoke();
                            exception = syncContext.WaitForCompletion();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        finally
                        {
                            SynchronizationContext.SetSynchronizationContext(oldSyncContext);
                            @event.Set();
                        }
                    }
                });

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!@event.WaitOne(this.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                }

                if (exception != null)
                {
                    ExceptionUtility.RethrowWithNoStackTraceLoss(exception);
                }
            }
            finally
            {
                foreach (var disposable in this.ExtractDisposables)
                {
                    CurrentScenario.AddTeardown(() => disposable.Dispose());
                }

                foreach (var teardown in this.Teardowns)
                {
                    CurrentScenario.AddTeardown(teardown);
                }
            }
        }

        /// <summary>
        /// Implementation from xUnit 2.0.
        /// </summary>
        private class AsyncTestSyncContext : SynchronizationContext, IDisposable
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

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Capturing an re-throwing an exception from another thread.")]
            public override void Send(SendOrPostCallback d, object state)
            {
                Guard.AgainstNullArgument("d", d);

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
}