// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    internal class Step : IStep
    {
        private readonly Sdk.Step step;

        public Step(string text, Action body)
        {
            this.step = CurrentScenario.AddStep(text, body);
        }

        public Step(string text, Action<IStepContext> body)
        {
            this.step = new StepContext(text, body).Step;
        }

        public Step(string text, Func<Task> body)
        {
            this.step = CurrentScenario.AddStep(text, body);
        }

        public Step(string text, Func<IStepContext, Task> body)
        {
            this.step = new StepContext(text, body).Step;
        }

        public IStep And()
        {
            return this;
        }

        public IStep WithTimeout(int millisecondsTimeout)
        {
            this.step.MillisecondsTimeout = millisecondsTimeout;
            return this;
        }

        public IStep InIsolation()
        {
            this.step.InIsolation = true;
            return this;
        }

        public IStep Skip(string reason)
        {
            this.step.SkipReason = reason;
            return this;
        }

        public IStep Teardown(Action teardown)
        {
            this.step.AddTeardown(teardown);
            return this;
        }
    }
}
