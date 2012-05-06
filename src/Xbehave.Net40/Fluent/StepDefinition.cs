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
            return ("When " + message)._(step, false, null);
        }

        public IStepDefinition When(string message, Action step)
        {
            return ("When " + message)._(step, false, null);
        }

        public IStepDefinition When(string message, Func<IEnumerable<IDisposable>> step)
        {
            return ("When " + message)._(step, false, null);
        }

        public IStepDefinition When(string message, Action step, Action dispose)
        {
            return ("When " + message)._(step, dispose, false, null);
        }

        public IStepDefinition Then(string message, Action step, bool inIsolation = false, string skip = null)
        {
            return ("Then " + message)._(step, inIsolation, skip);
        }

        public IStepDefinition ThenInIsolation(string message, Action step)
        {
            return ("Then in isolation " + message)._(step, true, null);
        }

        public IStepDefinition ThenSkip(string message, string reason, Action step)
        {
            return ("Then skip " + message)._(step, false, reason);
        }

        public IStepDefinition And(string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return ("and " + message)._(step, inIsolation, skip);
        }

        public IStepDefinition And(string message, Action step, bool inIsolation = false, string skip = null)
        {
            return message._(step, inIsolation, skip);
        }

        public IStepDefinition And(string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return message._(step, inIsolation, skip);
        }

        public IStepDefinition And(string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return message._(step, dispose, inIsolation, skip);
        }

        public IStepDefinition But(string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return message._(step, inIsolation, skip);
        }

        public IStepDefinition But(string message, Action step, bool inIsolation = false, string skip = null)
        {
            return message._(step, inIsolation, skip);
        }

        public IStepDefinition But(string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return message._(step, inIsolation, skip);
        }

        public IStepDefinition But(string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return message._(step, dispose, inIsolation, skip);
        }
    }
}
