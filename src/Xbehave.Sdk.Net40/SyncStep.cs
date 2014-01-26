// <copyright file="SyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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

        public override void Execute()
        {
            var teardowns = Enumerable.Empty<Action>();

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
                            teardowns = CurrentScenario.ExtractTeardowns();
                            SynchronizationContext.SetSynchronizationContext(oldSyncContext);
                            @event.Set();
                        }
                    }
                });

                if (this.MillisecondsTimeout > 0)
                {
                    // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                    if (!@event.WaitOne(this.MillisecondsTimeout, false))
                    {
                        throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                    }
                }
                else
                {
                    @event.WaitOne();
                }

                if (exception != null)
                {
                    ExceptionUtility.RethrowWithNoStackTraceLoss(exception);
                }
            }
            finally
            {
                foreach (var teardown in teardowns.Concat(this.Teardowns))
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
}