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
            var oldSyncContext = SynchronizationContext.Current;

            try
            {
                using (var asyncSyncContext = new AsyncTestSyncContext())
                {
                    SynchronizationContext.SetSynchronizationContext(asyncSyncContext);

                    ManualResetEvent @event = new ManualResetEvent(false);

                    if (this.MillisecondsTimeout > 0)
                    {
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            this.ExecuteMethodInfo();
                            @event.Set();
                        });

                        // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                        if (!@event.WaitOne(this.MillisecondsTimeout, false))
                        {
                            throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                        }
                    }
                    else
                    {
                        this.ExecuteMethodInfo();
                    }

                    var ex = asyncSyncContext.WaitForCompletion();
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldSyncContext);
                this.teardowns.ForEach(CurrentScenario.AddTeardown);
            }
        }
    }
}
