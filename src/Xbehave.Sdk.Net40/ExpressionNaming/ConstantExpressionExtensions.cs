// <copyright file="ConstantExpressionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class ConstantExpressionExtensions
    {
        public static IEnumerable<string> ToTokens(this ConstantExpression expression)
        {
            if (expression == null)
            {
                yield break;
            }

            Type type;

            if (expression.Value == null)
            {
                yield return "null";
            }
            else if (expression.Value is long)
            {
                yield return expression.Value.ToString().ToToken() + "L";
            }
            else if (expression.Value is ulong)
            {
                yield return expression.Value.ToString().ToToken() + "UL";
            }
            else if (expression.Value is uint)
            {
                yield return expression.Value.ToString().ToToken() + "U";
            }
            else if (expression.Value is float)
            {
                yield return expression.Value.ToString().ToToken() + "F";
            }
            else if (expression.Value is double)
            {
                yield return expression.Value.ToString().ToToken() + "D";
            }
            else if (expression.Value is decimal)
            {
                yield return expression.Value.ToString().ToToken() + "M";
            }
            else if (expression.Value is char)
            {
                yield return string.Concat("'", expression.Value.ToString().ToToken(), "'");
            }
            else if (expression.Value is string)
            {
                yield return string.Concat("\"", expression.Value.ToString().ToToken(), "\"");
            }
            else if ((type = expression.Value as Type) != null)
            {
                yield return type.Name.ToToken();
            }
            else
            {
                yield return expression.Value.ToString().ToToken();
            }
        }
    }
}
