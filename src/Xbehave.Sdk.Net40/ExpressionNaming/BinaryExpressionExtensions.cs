// <copyright file="BinaryExpressionExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class BinaryExpressionExtensions
    {
        public static IEnumerable<string> SelectNaturalLanguageTokens(this BinaryExpression expression)
        {
            if (expression == null)
            {
                yield break;
            }

            foreach (var token in expression.Right.SelectNaturalLanguageTokens())
            {
                yield return token;
            }

            yield return expression.NodeType.ToToken();

            foreach (var token in expression.Left.SelectNaturalLanguageTokens())
            {
                yield return token;
            }
        }
    }
}
