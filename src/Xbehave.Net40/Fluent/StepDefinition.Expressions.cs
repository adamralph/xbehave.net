// <copyright file="StepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Linq.Expressions;
    using Xbehave.Sdk.ExpressionNaming;
    using Xbehave.Sdk.Infrastructure;

    internal partial class StepDefinition : IStepDefinition
    {
        public static IStepDefinition Create(string stepType, Expression<Action> body)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);

            return StepDefinition.Create(stepType, body.Body.ToSentence(), body.Compile());
        }

        public IStepDefinition When(Expression<Action> body)
        {
            return Create("When", body);
        }

        public IStepDefinition Then(Expression<Action> body)
        {
            return Create("Then", body);
        }

        public IStepDefinition And(Expression<Action> body)
        {
            return Create("And", body);
        }

        public IStepDefinition But(Expression<Action> body)
        {
            return Create("But", body);
        }
    }
}
