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

        public static IStepDefinition Create(string stepType, string text, Action body)
        {
            return new StepDefinition(CurrentScenario.AddStep(stepType, text, body));
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

        public IStepDefinition Teardown(Action teardown)
        {
            this.createdStep.AddTeardown(teardown);
            return this;
        }
    }
}
