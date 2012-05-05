// <copyright file="QueueExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class QueueExtensions
    {
        // NOTE: not something I'd ever expose publicly, i.e. a mutating iterator
        public static IEnumerable<T> DequeueAll<T>(this Queue<T> source)
        {
            while (source.Any())
            {
                yield return source.Dequeue();
            }
        }

        public static T EnqueueAndReturn<T>(this Queue<T> destination, T item)
        {
            destination.Enqueue(item);
            return item;
        }
    }
}
