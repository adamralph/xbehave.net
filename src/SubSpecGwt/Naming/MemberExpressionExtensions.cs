// <copyright file="MemberExpressionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec.Naming
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal static class MemberExpressionExtensions
    {
        public static IEnumerable<string> ToTokens(this MemberExpression expression)
        {
            if (expression == null)
            {
                yield break;
            }

            {
                var token = expression.Member.Name.ToToken();
                yield return
                    expression.Expression != null && expression.Expression.Type.IsCompilerGenerated()
                    ? "the " + token
                    : token;
            }

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                if (expression.Expression == null &&
                    !expression.Member.DeclaringType.IsCompilerGenerated() &&
                    !expression.Member.DeclaringType.IsIgnored())
                {
                    yield return expression.Member.DeclaringType.Name.ToToken();
                }
                else if ((expression.Expression as MemberExpression) != null ||
                    (expression.Expression as MethodCallExpression) != null)
                {
                    foreach (var token in expression.Expression.ToTokens())
                    {
                        yield return token;
                    }
                }
            }
        }
    }
}
