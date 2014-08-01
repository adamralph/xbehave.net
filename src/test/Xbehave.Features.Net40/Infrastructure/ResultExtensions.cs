// <copyright file="ResultExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Features.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;

    public static class ResultExtensions
    {
        public static string ToDisplayString(this Result[] results, string header = null)
        {
            var text = string.Join(
                Environment.NewLine, results.Select((result, index) => Format(result, index)).ToArray());

            return header == null
                ? text
                : string.Concat(header, Environment.NewLine, text);
        }

        private static string Format(Result result, int index)
        {
            return string.Format("Result {0}: {1}", (++index).ToString(CultureInfo.InvariantCulture), result);
        }
    }
}
