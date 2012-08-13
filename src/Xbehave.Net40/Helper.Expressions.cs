// <copyright file="Helper.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Linq.Expressions;
    using Xbehave.Sdk.ExpressionNaming;
    using Xbehave.Sdk.Infrastructure;

    internal static partial class Helper
    {
        public static Fluent.IStep AddStep(string prefix, Expression<Action> body)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);

            return AddStep(string.Concat(prefix, body.Body.ToSentence()), body.Compile());
        }
    }
}
