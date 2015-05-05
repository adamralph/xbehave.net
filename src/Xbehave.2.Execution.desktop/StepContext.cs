// <copyright file="StepContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Sdk;

    public class StepContext : IStepContext
    {
        private readonly IStep step;
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public StepContext(IStep step)
        {
            this.step = step;
        }

        public IStep Step
        {
            get { return this.step; }
        }

        public IReadOnlyList<IDisposable> Disposables
        {
            get { return this.disposables; }
        }

        public IStepContext Using(IDisposable disposable)
        {
            if (disposable != null)
            {
                this.disposables.Add(disposable);
            }

            return this;
        }
    }
}
