// <copyright file="SyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading;

    public class SyncStep : Step
    {
        public SyncStep(string name, Action body, object stepType)
            : base(name, body.Method, body.Target, stepType)
        {
        }

        public override void Execute()
        {
            try
            {
                Exception ex = null;

                ManualResetEvent @event = new ManualResetEvent(false);

                ThreadPool.QueueUserWorkItem(o =>
                {
                    var oldSyncContext = SynchronizationContext.Current;
                    using (var asyncSyncContext = new AsyncTestSyncContext())
                    {
                        try
                        {
                            SynchronizationContext.SetSynchronizationContext(asyncSyncContext);

                            this.ExecuteMethodInfo();

                            ex = asyncSyncContext.WaitForCompletion();
                        }
                        finally
                        {
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

                if (ex != null)
                {
                    throw ex;
                }
            }
            finally
            {
                this.teardowns.ForEach(CurrentScenario.AddTeardown);
            }
        }
    }
}