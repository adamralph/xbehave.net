// <copyright file="ExpressionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal static class ExpressionExtensions
    {
        public static string ToStepName(this Expression expression)
        {
            return expression.ToStepName(" ");
        }

        public static string ToStepName(this Expression expression, string delimiter)
        {
            return string.Join(delimiter, expression.ToTokens().Reverse().ToArray());
        }

        public static IEnumerable<string> ToTokens(this Expression expression)
        {
            if (expression == null)
            {
                yield break;
            }

            var knownType = false;
            var methodCall = expression as MethodCallExpression;
            var member = expression as MemberExpression;
            var binary = expression as BinaryExpression;
            var constant = expression as ConstantExpression;
            var unary = expression as UnaryExpression;
            var lambda = expression as LambdaExpression;
            foreach (var token in
                methodCall.ToTokens()
                .Concat(member.ToTokens())
                .Concat(binary.ToTokens())
                .Concat(constant.ToTokens())
                .Concat(unary.ToTokens())
                .Concat(lambda.ToTokens()))
            {
                knownType = true;
                yield return token;
            }

            if (!knownType)
            {
                yield return string.Concat("(", expression.ToString(), "?)");
            }
        }
    }
}
