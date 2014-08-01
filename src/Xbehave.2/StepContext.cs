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
        private readonly StepDefinition step;

        public StepContext(string text, Action<IStepContext> body)
        {
            Guard.AgainstNullArgument("body", body);

            this.step = CurrentScenario.AddStep(text, () => body(this));
        }

        public StepContext(string text, Func<IStepContext, Task> body)
        {
            Guard.AgainstNullArgument("body", body);

            this.step = CurrentScenario.AddStep(text, () => body(this));
        }

        public StepDefinition Step
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
