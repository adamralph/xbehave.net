// <copyright file="StepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Linq.Expressions;

    internal partial class StepDefinition
    {
        [Obsolete("Use Then().Skip() instead.")]
        public IStepDefinition ThenSkip(Expression<Action> body)
        {
            return _.ThenSkip(body);
        }

        [Obsolete("Use Then().InIsolation() instead.")]
        public IStepDefinition ThenInIsolation(Expression<Action> body)
        {
            return _.ThenInIsolation(body);
        }
    }
}
