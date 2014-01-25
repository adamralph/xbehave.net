// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Xunit.Sdk;

    /// <summary>
    /// Provides the implementation to execute each step.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public abstract partial class Step
    {
        protected readonly List<Action> Teardowns = new List<Action>();
        private readonly string name;
        private readonly object stepType;

        public Step(string name, object stepType)
        {
            this.name = name;
            this.stepType = stepType;
        }

        public virtual string Name
        {
            get { return this.name; }
        }

        public object StepType
        {
            get { return this.stepType; }
        }

        public string SkipReason { get; set; }

        public bool InIsolation { get; set; }

        public int MillisecondsTimeout { get; set; }

        public void AddTeardown(Action teardown)
        {
            if (teardown != null)
            {
                this.Teardowns.Add(teardown);
            }
        }

        public void Execute()
        {
            IEnumerable<Action> teardowns = Enumerable.Empty<Action>();

            try
            {
                Exception ex = null;

                ManualResetEvent @event = new ManualResetEvent(false);

                ThreadPool.QueueUserWorkItem(o =>
                {
                    var oldSyncContext = SynchronizationContext.Current;

                    try
                    {
                        this.SetupSynchronizationContext();

                        this.ExecuteBody();

                        ex = this.WaitForCompletion();
                    }
                    catch (TargetInvocationException targetEx)
                    {
                        ex = targetEx.InnerException;
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }
                    finally
                    {
                        teardowns = CurrentScenario.ExtractTeardowns();
                        this.TeardownSynchronizationContext(oldSyncContext);

                        @event.Set();
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

        protected abstract void ExecuteBody();

        protected abstract Exception WaitForCompletion();

        protected virtual void SetupSynchronizationContext()
        {
        }

        protected virtual void TeardownSynchronizationContext(SynchronizationContext oldSyncContext)
        {
        }
    }
}