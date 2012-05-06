// <copyright file="StepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Linq.Expressions;

    internal partial class StepDefinition : IStepDefinition
    {
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

        public IStepDefinition ThenSkip(Expression<Action> assert, string reason)
        {
            return _.ThenSkip(assert, reason);
        }
    }
}
