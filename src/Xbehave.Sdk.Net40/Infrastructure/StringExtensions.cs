// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;

    public static class StringExtensions
    {
        public static string CompressWhitespace(this string source)
        {
            var spacesOnly = new string(source.Select(x => char.IsWhiteSpace(x) ? ' ' : x).ToArray());
            var words = spacesOnly.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words);
        }

        public static string MergeOrdinalIgnoreCase(this string source, string text)
        {
            if (text == null)
            {
                return source;
            }

            if (source == null)
            {
                return text;
            }

            if (text.StartsWith(source, StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }

            return string.Concat(source, text);
        }

        public static string AttemptFormatInvariantCulture(this string format, params object[] args)
        {
            try
            {
                return string.Format(CultureInfo.InvariantCulture, format, args);
            }
            catch (FormatException)
            {
                return format;
            }
        }
    }
}
