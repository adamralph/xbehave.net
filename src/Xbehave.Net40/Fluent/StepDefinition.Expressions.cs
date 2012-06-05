// <copyright file="StepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Linq.Expressions;

    internal partial class StepDefinition : IStepDefinition
    {
        public IStepDefinition When(Expression<Action> body, Action teardown = null)
        {
            return _.When(body, teardown);
        }

        public IStepDefinition Then(Expression<Action> body, Action teardown = null)
        {
            return _.Then(body, teardown);
        }

        public IStepDefinition And(Expression<Action> body, Action teardown = null)
        {
            return _.And(body, teardown);
        }

        public IStepDefinition But(Expression<Action> body, Action teardown = null)
        {
            return _.But(body, teardown);
        }
    }
}
