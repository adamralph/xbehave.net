// <copyright file="TestResultMessageExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Xunit.Abstractions;

    public static class TestResultMessageExtensions
    {
        public static string ToDisplayString(this ITestResultMessage[] results, string header)
        {
            var text = string.Join(
                Environment.NewLine, results.Select(Format).ToArray());

            return string.Concat(header, Environment.NewLine, text);
        }

        private static string Format(ITestResultMessage result, int index)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Result {0}: {1}",
                (++index).ToString(CultureInfo.InvariantCulture),
                result);
        }
    }
}
