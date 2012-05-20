// <copyright file="EnumerableExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            Require.NotNull(source, "source");

            foreach (var t in source)
            {
                yield return t;
            }

            yield return item;
        }
    }
}
