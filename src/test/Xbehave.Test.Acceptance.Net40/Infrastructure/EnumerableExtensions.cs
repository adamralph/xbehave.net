// <copyright file="EnumerableExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            return new Queue<T>(source);
        }
    }
}
