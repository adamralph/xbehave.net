// <copyright file="QueueExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class QueueExtensions
    {
        // NOTE: in practice, we could just pass Queue as IEnumerable since it enumerates in forward order as required, but this behaviour is undocumented
        // NOTE: not something I'd ever expose publicly, i.e. a mutating iterator
        public static IEnumerable<T> DequeueAll<T>(this Queue<T> source)
        {
            while (source.Any())
            {
                yield return source.Dequeue();
            }
        }
    }
}
