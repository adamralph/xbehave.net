// <copyright file="StepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Linq.Expressions;
    using Xbehave.Internal;

    internal class StepDefinition : IStepDefinition
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

        public IStepDefinition When(Expression<Action> act)
        {
            return _.When(act);
        }

        public IStepDefinition Then(Expression<Action> assert)
        {
            return _.Then(assert);
        }

        public IStepDefinition ThenInIsolation(Expression<Action> assert)
        {
            return _.ThenInIsolation(assert);
        }

        public IStepDefinition ThenSkip(Expression<Action> assert)
        {
            return _.ThenSkip(assert);
        }
    }
}
