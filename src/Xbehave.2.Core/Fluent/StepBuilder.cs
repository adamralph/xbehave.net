// <copyright file="StepBuilder.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    internal class StepBuilder : IStepBuilder
    {
        private readonly StepDefinition step;

        public StepBuilder(string text, Func<IStepContext, Task> body)
        {
            this.step = ThreadStaticStepHub.CreateAndAdd(text, body);
        }

        public IStepBuilder Skip(string reason)
        {
            this.step.SkipReason = reason;
            return this;
        }

        public IStepBuilder Teardown(Action teardown)
        {
            this.step.Add(teardown);
            return this;
        }

        public IStepBuilder ContinueOnFailure()
        {
            this.step.ContinueOnFailure = true;
            return this;
        }
    }
}
