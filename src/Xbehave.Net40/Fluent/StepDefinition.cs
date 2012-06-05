// <copyright file="StepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using Xbehave.Sdk;

    internal partial class StepDefinition : IStepDefinition
    {
        private readonly Step createdStep;

        public StepDefinition(Step createdStep)
        {
            this.createdStep = createdStep;
        }

        public IStepDefinition WithTimeout(int millisecondsTimeout)
        {
            this.createdStep.MillisecondsTimeout = millisecondsTimeout;
            return this;
        }

        public IStepDefinition InIsolation()
        {
            this.createdStep.InIsolation = true;
            return this;
        }

        public IStepDefinition Skip(string reason)
        {
            this.createdStep.SkipReason = reason;
            return this;
        }

        public IStepDefinition When(string message, Action body, Action teardown = null)
        {
            return message.When(body, teardown);
        }

        public IStepDefinition Then(string message, Action body, Action teardown = null)
        {
            return message.Then(body, teardown);
        }

        public IStepDefinition And(string message, Action body, Action teardown = null)
        {
            return message.And(body, teardown);
        }

        public IStepDefinition But(string message, Action body, Action teardown = null)
        {
            return message.But(body, teardown);
        }
    }
}
