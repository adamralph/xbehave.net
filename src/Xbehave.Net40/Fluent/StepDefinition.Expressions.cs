// <copyright file="StepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal partial class StepDefinition : IStepDefinition
    {
        public IStepDefinition When(Expression<Action> step)
        {
            return _.When(step);
        }

        public IStepDefinition When(Expression<Func<IDisposable>> step)
        {
            return _.When(step);
        }

        public IStepDefinition When(Expression<Func<System.Collections.Generic.IEnumerable<IDisposable>>> step)
        {
            return _.When(step);
        }

        public IStepDefinition When(Expression<Action> step, Action dispose)
        {
            return _.When(step, dispose);
        }

        public IStepDefinition Then(Expression<Action> step)
        {
            return _.Then(step);
        }

        public IStepDefinition ThenInIsolation(Expression<Action> step)
        {
            return _.ThenInIsolation(step);
        }

        public IStepDefinition ThenSkip(Expression<Action> step, string reason)
        {
            return _.ThenSkip(step, reason);
        }

        public IStepDefinition And(Expression<Action> step)
        {
            return _.And(step);
        }

        public IStepDefinition And(Expression<Func<IDisposable>> step)
        {
            return _.And(step);
        }

        public IStepDefinition And(Expression<Func<IEnumerable<IDisposable>>> step)
        {
            return _.And(step);
        }

        public IStepDefinition And(Expression<Action> step, Action dispose)
        {
            return _.And(step, dispose);
        }

        public IStepDefinition AndInIsolation(Expression<Action> step)
        {
            return _.AndInIsolation(step);
        }

        public IStepDefinition AndSkip(Expression<Action> step, string reason)
        {
            return _.AndSkip(step, reason);
        }

        public IStepDefinition But(Expression<Action> step)
        {
            return _.But(step);
        }

        public IStepDefinition ButInIsolation(Expression<Action> step)
        {
            return _.ButInIsolation(step);
        }

        public IStepDefinition ButSkip(Expression<Action> step, string reason)
        {
            return _.ButSkip(step, reason);
        }
    }
}
