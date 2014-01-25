// <copyright file="AsyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncStep : Step
    {
        private readonly Func<Task> body;
        private Task task;

        public AsyncStep(string name, Func<Task> body, object stepType)
            : base(name, stepType)
        {
            this.body = body;
        }
        
        protected override void ExecuteBody()
        {
            this.task = this.body.Invoke();
        }

        protected override Exception WaitForCompletion()
        {
            try
            {
                this.task.Wait();
            }
            catch (AggregateException ae)
            {
                return ae.InnerException;
            }

            return null;
        }
    }
}
