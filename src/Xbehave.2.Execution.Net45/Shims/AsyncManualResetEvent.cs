// <copyright file="AsyncManualResetEvent.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Shims
{
    using System.Threading.Tasks;

    internal class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

        public AsyncManualResetEvent(bool signaled = false)
        {
            if (signaled)
            {
                this.taskCompletionSource.TrySetResult(true);
            }
        }

        public bool IsSet
        {
            get { return this.taskCompletionSource.Task.IsCompleted; }
        }

        public Task WaitAsync()
        {
            return this.taskCompletionSource.Task;
        }

        public void Set()
        {
            this.taskCompletionSource.TrySetResult(true);
        }

        public void Reset()
        {
            if (this.IsSet)
            {
                this.taskCompletionSource = new TaskCompletionSource<bool>();
            }
        }
    }
}
