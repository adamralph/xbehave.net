// <copyright file="QueueExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System.Collections.Generic;

    internal static class QueueExtensions
    {
        public static IEnumerable<T> Dequeue<T>(this Queue<T> queue, int count)
        {
            var array = new T[count];
            for (var index = 0; index < count; ++index)
            {
                array[index] = queue.Dequeue();
            }

            return array;
        }
    }
}
