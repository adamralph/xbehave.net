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

        public IStepDefinition When(Expression<Func<IDisposable>> body)
        {
            return _.When(body);
        }

        public IStepDefinition When(Expression<Func<System.Collections.Generic.IEnumerable<IDisposable>>> body)
        {
            return _.When(body);
        }

        public IStepDefinition When(Expression<Action> body, Action dispose)
        {
            return _.When(body, dispose);
        }

        public IStepDefinition Then(Expression<Action> body)
        {
            return _.Then(body);
        }

        public IStepDefinition And(Expression<Action> body)
        {
            return _.And(body);
        }

        public IStepDefinition And(Expression<Func<IDisposable>> body)
        {
            return _.And(body);
        }

        public IStepDefinition And(Expression<Func<IEnumerable<IDisposable>>> body)
        {
            return _.And(body);
        }

        public IStepDefinition And(Expression<Action> body, Action dispose)
        {
            return _.And(body, dispose);
        }

        public IStepDefinition But(Expression<Action> body)
        {
            return _.But(body);
        }
    }
}
