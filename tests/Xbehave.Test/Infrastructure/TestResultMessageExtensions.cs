namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Xunit.Abstractions;

    public static class TestResultMessageExtensions
    {
        public static string ToDisplayString(this ITestResultMessage[] results, string header) =>
            header + Environment.NewLine + string.Join(Environment.NewLine, results.Select(Format));

        private static string Format(ITestResultMessage result, int index) =>
            $"Result {(++index).ToString(CultureInfo.InvariantCulture)}: {result}";
    }
}
