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
        public SyncStep(string name, Action body, object stepType)
            : base(name, body.Method, body.Target, stepType)
        {
        }

        public override void Execute()
        {
            IEnumerable<Action> teardowns = Enumerable.Empty<Action>();

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
                        catch (TargetInvocationException targetEx)
                        {
                            ex = targetEx.InnerException;
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

                if (ex != null)
                {
                    ExceptionUtility.RethrowWithNoStackTraceLoss(ex);
                }
            }
            finally
            {
                foreach (var teardown in teardowns)
                {
                    CurrentScenario.AddTeardown(teardown);
                }

                this.Teardowns.ForEach(CurrentScenario.AddTeardown);
            }
        }
    }
}