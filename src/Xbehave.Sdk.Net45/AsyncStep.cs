// <copyright file="AsyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading.Tasks;
    using Xunit.Sdk;

    public class AsyncStep : Step
    {
        private readonly Func<Task> body;

        public AsyncStep(string name, Func<Task> body, object stepType)
            : base(name, stepType)
        {
            Guard.AgainstNullArgument("body", body);

            this.body = body;
        }

        public override void Execute()
        {
            try
            {
                if (this.MillisecondsTimeout > 0)
                {
                    var task = this.body();
                    var timeout = Task.Delay(this.MillisecondsTimeout);
                    if (Task.WhenAny(task, timeout).Result == timeout)
                    {
                        throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                    }

                    task.Wait();
                }
                else
                {
                    this.body().Wait();
                }
            }
            catch (AggregateException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }
            finally
            {
                foreach (var teardown in this.Teardowns)
                {
                    CurrentScenario.AddTeardown(teardown);
                }
            }
        }
    }
}
