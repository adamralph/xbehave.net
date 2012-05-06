// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System;
    using System.Linq;

    internal static class StringExtensions
    {
        public static string ToSentenceStartingWith(this string sentence, string words)
        {
            words = (words ?? string.Empty).ToSingleSpaceSentence();
            sentence = (sentence ?? string.Empty).ToSingleSpaceSentence();

            if (sentence.Equals(words, StringComparison.OrdinalIgnoreCase))
            {
                return words;
            }

            if (sentence.StartsWith(words, StringComparison.OrdinalIgnoreCase))
            {
                return string.Concat(words, sentence.Substring(words.Length));
            }

            return string.Concat(words, " ", sentence);
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
