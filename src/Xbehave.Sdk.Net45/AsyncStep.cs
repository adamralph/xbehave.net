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
                if (!this.body().Wait(this.MillisecondsTimeout))
                {
                    throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                }
            }
            catch (AggregateException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }
            finally
            {
                foreach (var disposable in this.ExtractDisposables)
                {
                    CurrentScenario.AddTeardown(() => disposable.Dispose());
                }

                foreach (var teardown in this.Teardowns)
                {
                    CurrentScenario.AddTeardown(teardown);
                }
            }
        }
    }
}
