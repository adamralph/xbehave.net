// <copyright file="StepContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    internal partial class StepContext : IStepContext
    {
        private Sdk.Step step;

        public void Assign(Sdk.Step step)
        {
            Guard.AgainstNullArgument("step", step);

            this.step = step;
        }

        public IStepContext Using(IDisposable disposable)
        {
            this.step.AddDisposable(disposable);
            return this;
        }
    }
}
