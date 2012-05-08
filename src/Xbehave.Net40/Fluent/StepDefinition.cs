// <copyright file="StepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
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

        public IStepDefinition When(string message, Func<IDisposable> body)
        {
            return message.When(body);
        }

        public IStepDefinition When(string message, Action body)
        {
            return message.When(body);
        }

        public IStepDefinition When(string message, Func<IEnumerable<IDisposable>> body)
        {
            return message.When(body);
        }

        public IStepDefinition When(string message, Action body, Action dispose)
        {
            return message.When(body, dispose);
        }

        public IStepDefinition Then(string message, Action body)
        {
            return message.Then(body);
        }

        public IStepDefinition ThenInIsolation(string message, Action body)
        {
            return message.ThenInIsolation(body);
        }

        public IStepDefinition ThenSkip(string message, Action body, string reason)
        {
            return message.ThenSkip(body, reason);
        }

        public IStepDefinition And(string message, Func<IDisposable> body)
        {
            return message.And(body);
        }

        public IStepDefinition And(string message, Action body)
        {
            return message.And(body);
        }

        public IStepDefinition And(string message, Func<IEnumerable<IDisposable>> body)
        {
            return message.And(body);
        }

        public IStepDefinition And(string message, Action body, Action dispose)
        {
            return message.And(body, dispose);
        }

        public IStepDefinition AndInIsolation(string message, Action body)
        {
            return message.AndInIsolation(body);
        }

        public IStepDefinition AndSkip(string message, Action body, string reason)
        {
            return message.AndSkip(body, reason);
        }

        public IStepDefinition But(string message, Action body)
        {
            return message.But(body);
        }

        public IStepDefinition ButInIsolation(string message, Action body)
        {
            return message.ButInIsolation(body);
        }

        public IStepDefinition ButSkip(string message, Action body, string reason)
        {
            return message.ButSkip(body, reason);
        }
    }
}
