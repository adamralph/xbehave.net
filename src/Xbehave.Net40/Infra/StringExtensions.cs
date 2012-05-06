// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System;
    using System.Linq;

    internal static class StringExtensions
    {
        public static string ToSentenceStartingWith(this string sentence, string text)
        {
            text = (text ?? string.Empty).ToSingleSpaceSentence();
            sentence = (sentence ?? string.Empty).ToSingleSpaceSentence();

            if (sentence.Equals(text, StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }

            if (sentence.StartsWith(text, StringComparison.OrdinalIgnoreCase))
            {
                return string.Concat(text, " ", sentence.Substring(text.Length));
            }

            return string.Concat(text, " ", sentence);
        }

        public static string ToSingleSpaceSentence(this string text)
        {
            return string.Join(" ", text.ToWords());
        }

        public static string[] ToWords(this string text)
        {
            return new string(text.Select(x => char.IsWhiteSpace(x) ? ' ' : x).ToArray()).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
