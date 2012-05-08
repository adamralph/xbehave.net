// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    internal static class StringExtensions
    {
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "The result is used for display purposes only.")]
        public static string ToToken(this string text)
        {
            return Regex.Replace(text, "([A-Z])", " $1", RegexOptions.Compiled).Trim().ToLowerInvariant();
        }

        public static IEnumerable<string> AddListSeperators(this IEnumerable<string> items)
        {
            var list = new List<string>(items);
            for (var index = 0; index < list.Count; ++index)
            {
                if (index < list.Count - 2)
                {
                    yield return list[index] + ",";
                }
                else if (index == list.Count - 2)
                {
                    yield return list[index] + " and";
                }
                else
                {
                    yield return list[index];
                }
            }
        }
    }
}
