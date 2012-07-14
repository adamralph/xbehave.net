// <copyright file="UnaryExpressionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class UnaryExpressionExtensions
    {
        public static IEnumerable<string> ToTokens(this UnaryExpression expression)
        {
            if (expression == null)
            {
                yield break;
            }

            if (expression.NodeType == ExpressionType.TypeAs ||
                expression.NodeType == ExpressionType.Convert ||
                expression.NodeType == ExpressionType.ConvertChecked)
            {
                yield return string.Concat(
                    string.Join(" ", expression.Operand.ToTokens().Reverse().ToArray()),
                    " ",
                    expression.NodeType.ToToken(),
                    " ",
                    expression.Type.Name.ToToken());
            }
            else if (expression.NodeType == ExpressionType.Quote)
            {
                yield return string.Concat(
                    "(",
                    string.Join(" ", expression.Operand.ToTokens().Reverse().ToArray()),
                    ")");
            }
            else
            {
                foreach (var token in expression.Operand.ToTokens())
                {
                    yield return token;
                }

                yield return expression.NodeType.ToToken();
            }
        }
    }
}
