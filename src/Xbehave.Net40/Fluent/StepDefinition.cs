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
        private readonly Step step;

        public StepDefinition(Step step)
        {
            this.step = step;
        }

        public IStepDefinition WithTimeout(int millisecondsTimeout)
        {
            this.step.MillisecondsTimeout = millisecondsTimeout;
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

        public IStepDefinition ThenSkip(string message, string reason, Action step)
        {
            return message.ThenSkip(reason, step);
        }

        public IStepDefinition And(string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return message.And(step, inIsolation, skip);
        }

        public IStepDefinition And(string message, Action step, bool inIsolation = false, string skip = null)
        {
            return message.And(step, inIsolation, skip);
        }

        public IStepDefinition And(string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return message.And(step, inIsolation, skip);
        }

        public IStepDefinition And(string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return message.And(step, dispose, inIsolation, skip);
        }

        public IStepDefinition But(string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return message.But(step, inIsolation, skip);
        }

        public IStepDefinition But(string message, Action step, bool inIsolation = false, string skip = null)
        {
            return message.But(step, inIsolation, skip);
        }

        public IStepDefinition But(string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return message.But(step, inIsolation, skip);
        }

        public IStepDefinition But(string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return message.But(step, dispose, inIsolation, skip);
        }
    }
}
