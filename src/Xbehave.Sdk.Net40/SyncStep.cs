// <copyright file="SyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading;

    public class SyncStep : Step
    {
        private readonly Action body;
        private AsyncTestSyncContext syncContext;

        public SyncStep(string name, Action body, object stepType)
            : base(name, stepType)
        {
            this.body = body;
        }

        protected override void SetupSynchronizationContext()
        {
            this.syncContext = new AsyncTestSyncContext();
            SynchronizationContext.SetSynchronizationContext(this.syncContext);
        }

        protected override void TeardownSynchronizationContext(SynchronizationContext oldSyncContext)
        {
            SynchronizationContext.SetSynchronizationContext(oldSyncContext);
            this.syncContext.Dispose();
        }

        protected override void ExecuteBody()
        {
            this.body.Invoke();
        }

        protected override Exception WaitForCompletion()
        {
            return this.syncContext.WaitForCompletion();
        }
    }
}