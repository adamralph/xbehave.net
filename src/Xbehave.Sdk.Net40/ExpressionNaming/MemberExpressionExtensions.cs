// <copyright file="MemberExpressionExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class MemberExpressionExtensions
    {
        public static IEnumerable<string> SelectNaturalLanguageTokens(this MemberExpression expression)
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
                if (
                    expression.Expression == null &&
                    expression.Member.DeclaringType != null &&
                    !expression.Member.DeclaringType.IsCompilerGenerated() &&
                    !expression.Member.DeclaringType.IsIgnored())
                {
                    yield return expression.Member.DeclaringType.Name.ToToken();
                }
                else if ((expression.Expression as MemberExpression) != null ||
                    (expression.Expression as MethodCallExpression) != null)
                {
                    foreach (var token in expression.Expression.SelectNaturalLanguageTokens())
                    {
                        yield return token;
                    }
                }
            }
        }
    }
}
