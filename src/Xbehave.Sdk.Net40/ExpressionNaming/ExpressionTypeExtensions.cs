// <copyright file="ExpressionTypeExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class ExpressionTypeExtensions
    {
        private static readonly Dictionary<ExpressionType, string> Tokens = new Dictionary<ExpressionType, string>
        {
            // binary
            { ExpressionType.Add, "plus" },
            { ExpressionType.AddChecked, "plus" },
            { ExpressionType.And, "and" },
            { ExpressionType.AndAlso, "and" },
            { ExpressionType.ArrayIndex, "item" },
            { ExpressionType.Coalesce, "coalesced with" },
            { ExpressionType.Divide, "divided by" },
            { ExpressionType.Equal, "equals" },
            { ExpressionType.ExclusiveOr, "xor" },
            { ExpressionType.GreaterThan, "is greater than" },
            { ExpressionType.GreaterThanOrEqual, "is greater than or equal to" },
            { ExpressionType.LeftShift, "left shifted by" },
            { ExpressionType.LessThan, "is less than" },
            { ExpressionType.LessThanOrEqual, "is less than or equal to" },
            { ExpressionType.Modulo, "modulo" },
            { ExpressionType.Multiply, "multiplied by" },
            { ExpressionType.MultiplyChecked, "multiplied by" },
            { ExpressionType.NotEqual, "does not equal" },
            { ExpressionType.Or, "or" },
            { ExpressionType.OrElse, "or" },
            { ExpressionType.Power, "to the power of" },
            { ExpressionType.RightShift, "right shifted by" },
            { ExpressionType.Subtract, "minus" },
            { ExpressionType.SubtractChecked, "minus" },

            // unary
            { ExpressionType.ArrayLength, "the length of" },
            { ExpressionType.Convert, "converted to" },
            { ExpressionType.ConvertChecked, "converted to" },
            { ExpressionType.Negate, "the negative of " },
            { ExpressionType.NegateChecked, "the negative of" },
            { ExpressionType.Not, "not" },
            { ExpressionType.Quote, string.Empty },
            { ExpressionType.TypeAs, "as" },
            { ExpressionType.UnaryPlus, "the positive of" },
        };

        public static string ToToken(this ExpressionType type)
        {
            string token;
            if (Tokens.TryGetValue(type, out token))
            {
                return token;
            }

            return type.ToString().ToToken();
        }
    }
}
