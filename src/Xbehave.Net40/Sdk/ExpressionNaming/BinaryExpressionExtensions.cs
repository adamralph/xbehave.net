// <copyright file="BinaryExpressionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal static class BinaryExpressionExtensions
    {
        public static IEnumerable<string> ToTokens(this BinaryExpression expression)
        {
            if (expression == null)
            {
                yield break;
            }

            foreach (var token in expression.Right.ToTokens())
            {
                yield return token;
            }

            yield return expression.NodeType.ToToken();

            foreach (var token in expression.Left.ToTokens())
            {
                yield return token;
            }
        }
    }
}
