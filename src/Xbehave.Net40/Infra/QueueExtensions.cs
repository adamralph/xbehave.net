// <copyright file="QueueExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class QueueExtensions
    {
        public static IEnumerable<T> DequeueAll<T>(this Queue<T> source)
        {
            while (source.Any())
            {
                yield return source.Dequeue();
            }
        }
    }
}
