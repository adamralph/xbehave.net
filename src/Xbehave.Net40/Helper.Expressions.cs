// <copyright file="Helper.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Linq.Expressions;
    using Xbehave.Sdk;
    using Xbehave.Sdk.ExpressionNaming;

    internal static partial class Helper
    {
        public static Fluent.IStep AddStep(string prefix, Expression<Action> body)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);

            return AddStep(string.Concat(prefix, body.Body.ToNaturalLanguage()), body.Compile());
        }
    }
}
