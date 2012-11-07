// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;

    internal partial class Step : IStep
    {
        private readonly Sdk.Step step;

        public Step(Sdk.Step step)
        {
            this.step = step;
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
