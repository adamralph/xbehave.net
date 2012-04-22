// <copyright file="StaticReflection.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class StaticReflection
    {
        public static MethodInfo MethodOf(Expression<Action> expression)
        {
            var body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
    }
}
