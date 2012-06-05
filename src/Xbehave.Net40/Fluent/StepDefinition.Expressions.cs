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
        public IStepDefinition When(Expression<Action> body)
        {
            return _.When(body);
        }

        public IStepDefinition Then(Expression<Action> body)
        {
            return _.Then(body);
        }

        public IStepDefinition And(Expression<Action> body)
        {
            return _.And(body);
        }

        public IStepDefinition But(Expression<Action> body)
        {
            return _.But(body);
        }
    }
}
