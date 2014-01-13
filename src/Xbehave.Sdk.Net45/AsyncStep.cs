// <copyright file="AsyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading.Tasks;

    class AsyncStep : Step
    {
        public AsyncStep(string name, Func<Task> body, object stepType)
            : base(name, body.Method, body.Target, stepType)
        {
        }

        public override void Execute()
        {
            try
            {
                var bodyTask = (Task)this.ExecuteMethodInfo();

                if (this.MillisecondsTimeout > 0)
                {
                    var delay = Task.Delay(this.MillisecondsTimeout);
                    var firstToFinish = Task.WhenAny(delay, bodyTask).Result;

                    if (firstToFinish == delay)
                    {
                        throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                    }
                }
                else
                {
                    bodyTask.Wait();
                }
            }
            catch (AggregateException ae)
            {
                var innerException = ae.InnerException;
                throw innerException;
            }
            finally
            {
                this.teardowns.ForEach(CurrentScenario.AddTeardown);
            }
        }
    }
}
