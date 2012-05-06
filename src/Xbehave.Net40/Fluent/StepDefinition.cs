// <copyright file="StepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Internal;

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

        public IStepDefinition When(string message, Func<IDisposable> step)
        {
            return message.When(step);
        }

        public IStepDefinition When(string message, Action step)
        {
            return message.When(step);
        }

        public IStepDefinition When(string message, Func<IEnumerable<IDisposable>> step)
        {
            return message.When(step);
        }

        public IStepDefinition When(string message, Action step, Action dispose)
        {
            return message.When(step, dispose);
        }

        public IStepDefinition Then(string message, Action step)
        {
            return message.Then(step);
        }

        public IStepDefinition ThenInIsolation(string message, Action step)
        {
            return message.ThenInIsolation(step);
        }

        public IStepDefinition ThenSkip(string message, Action step, string reason)
        {
            return message.ThenSkip(step, reason);
        }
        
        public IStepDefinition And(string message, Func<IDisposable> step)
        {
            return message.And(step);
        }

        public IStepDefinition And(string message, Action step)
        {
            return message.And(step);
        }

        public IStepDefinition And(string message, Func<IEnumerable<IDisposable>> step)
        {
            return message.And(step);
        }

        public IStepDefinition And(string message, Action step, Action dispose)
        {
            return message.And(step, dispose);
        }

        public IStepDefinition AndInIsolation(string message, Action step)
        {
            return message.AndInIsolation(step);
        }

        public IStepDefinition AndSkip(string message, Action step, string reason)
        {
            return message.AndSkip(step, reason);
        }

        public IStepDefinition But(string message, Action step)
        {
            return message.But(step);
        }

        public IStepDefinition ButInIsolation(string message, Action step)
        {
            return message.ButInIsolation(step);
        }

        public IStepDefinition ButSkip(string message, Action step, string reason)
        {
            return message.ButSkip(step, reason);
        }
    }
}
