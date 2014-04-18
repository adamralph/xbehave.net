// <copyright file="StepContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    internal class StepContext : IStepContext
    {
        private readonly Step step;

        public StepContext(string text, Action<IStepContext> body, StepType stepType)
        {
            Guard.AgainstNullArgument("body", body);

            this.step = CurrentScenario.AddStep(text, () => body(this), stepType);
        }

        public StepContext(string text, Func<IStepContext, Task> body, StepType stepType)
        {
            Guard.AgainstNullArgument("body", body);

            this.step = CurrentScenario.AddStep(text, () => body(this), stepType);
        }

        public Step Step
        {
            get { return this.step; }
        }

        public IStepContext Using(IDisposable disposable)
        {
            this.step.AddDisposable(disposable);
            return this;
        }
    }
}
