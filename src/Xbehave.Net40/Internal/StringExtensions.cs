// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Fluent;
    using Xbehave.Infra;

    internal static partial class StringExtensions
    {
        public static IStepDefinition _(this string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(step), inIsolation, skip)));
        }

        public static IStepDefinition _(this string message, Action step, bool inIsolation = false, string skip = null)
        {
            return _(message, DisposableFunctionFactory.Create(step), inIsolation, skip);
        }

        public static IStepDefinition _(this string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return _(message, DisposableFunctionFactory.Create(step), inIsolation, skip);
        }

        public static IStepDefinition _(this string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return _(message, DisposableFunctionFactory.Create(step, dispose), inIsolation, skip);
        }

        public static string ToStepMessage(this string message, string firstWord)
        {
            firstWord = (firstWord ?? string.Empty).Trim().ToUpperInvariantInitial();
            var words = (message ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Trim())
                .Where(word => word.Length != 0)
                .SkipWhile(word => word.Equals(firstWord, StringComparison.OrdinalIgnoreCase));

            return string.Join(" ", firstWord.ToString().AsEnumerable().Concat(words).ToArray());
        }

        private static string ToUpperInvariantInitial(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpperInvariant();
            }

            return text.Substring(0, 1).ToUpperInvariant() + text.Substring(1);
        }
    }
}
