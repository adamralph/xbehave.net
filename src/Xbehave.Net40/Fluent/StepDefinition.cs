// <copyright file="StepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;
    using Xbehave.Sdk;

    internal partial class StepDefinition : IStepDefinition
    {
        private readonly Step createdStep;

        public StepDefinition(Step createdStep)
        {
            this.createdStep = createdStep;
        }

        public IStepDefinition And()
        {
            return this;
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

        public IStepDefinition When(string message, Action body)
        {
            return message.When(body);
        }

        public IStepDefinition Then(string message, Action body)
        {
            return message.Then(body);
        }

        public IStepDefinition And(string message, Action body)
        {
            return message.And(body);
        }

        public IStepDefinition But(string message, Action body)
        {
            return message.But(body);
        }

        public IStepDefinition Teardown(Action teardown)
        {
            this.createdStep.AddTeardown(teardown);
            return this;
        }
    }
}
