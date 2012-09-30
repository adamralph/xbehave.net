// <copyright file="ExpressionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class ExpressionExtensions
    {
        public static string ToNaturalLanguage(this Expression expression)
        {
            return expression.ToNaturalLanguage(" ");
        }

        // NOTE: for ad hoc testing in order to distinguish delimiters from spaces within tokens
        public static string ToNaturalLanguage(this Expression expression, string delimiter)
        {
            return string.Join(delimiter, expression.SelectNaturalLanguageTokens().Reverse().ToArray());
        }

        public static IEnumerable<string> SelectNaturalLanguageTokens(this Expression expression)
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
                methodCall.SelectNaturalLanguageTokens()
                .Concat(member.SelectNaturalLanguageTokens())
                .Concat(binary.SelectNaturalLanguageTokens())
                .Concat(constant.SelectNaturalLanguageTokens())
                .Concat(unary.SelectNaturalLanguageTokens())
                .Concat(lambda.SelectNaturalLanguageTokens()))
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
