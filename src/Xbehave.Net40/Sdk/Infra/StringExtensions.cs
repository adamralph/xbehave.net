// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System;
    using System.Globalization;
    using System.Linq;

    internal static class StringExtensions
    {
        public static string StartingWithOrdinalIgnoreCase(this string source, string text)
        {
            if (text == null)
            {
                return source;
            }

            if (source == null)
            {
                return text;
            }

            for (var i = text.Length; i > 0; --i)
            {
                var result = string.Concat(text, new string(source.Skip(i).ToArray()));
                if (result.EndsWith(source, StringComparison.OrdinalIgnoreCase))
                {
                    return result;
                }
            }

            return string.Concat(text, source);
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
