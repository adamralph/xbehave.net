// <copyright file="EnumerableExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infrastructure
{
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            Guard.AgainstNullArgument("source", source);

            foreach (var t in source)
            {
                yield return t;
            }

            // NOTE: see http://msmvps.com/blogs/jon_skeet/archive/2010/12/27/reimplementing-linq-to-objects-part-8-concat.aspx
            source = null;

            yield return item;
        }
    }
}
