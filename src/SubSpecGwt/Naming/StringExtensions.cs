// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec.Naming
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal static class StringExtensions
    {
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
