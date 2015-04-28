// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    internal class Step : IStep, IStepContext
    {
        private readonly Sdk.Step step;

        public Step(string text, Action body)
            : this(text, c => body())
        {
        }

        public Step(string text, Action<IStepContext> body)
        {
            this.step = ThreadStaticStepHub.CreateAndAdd(text, () => body(this));
        }

        public Step(string text, Func<Task> body)
            : this(text, c => body())
        {
        }

        public Step(string text, Func<IStepContext, Task> body)
        {
            this.step = ThreadStaticStepHub.CreateAndAdd(text, () => body(this));
        }

        public IStep Skip(string reason)
        {
            this.step.SkipReason = reason;
            return this;
        }

        public IStep Teardown(Action teardown)
        {
            this.step.Add(teardown);
            return this;
        }

        public IStep ContinueOnFailure()
        {
            this.step.ContinueOnFailure = true;
            return this;
        }

        public IStepContext Using(IDisposable disposable)
        {
            this.step.Add(disposable);
            return this;
        }
    }
}
