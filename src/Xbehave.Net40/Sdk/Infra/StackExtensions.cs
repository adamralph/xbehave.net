// <copyright file="StackExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class StackExtensions
    {
        // NOTE: in practice, we could just pass Stack as IEnumerable since it enumerates in reverse order as required, but this behaviour is undocumented
        // NOTE: not something I'd ever expose publicly, i.e. a mutating iterator
        public static IEnumerable<T> Unwind<T>(this Stack<T> source)
        {
            Require.NotNull(source, "source");

            while (source.Any())
            {
                yield return source.Pop();
            }
        }

        public static void PushIfNotNull<T>(this Stack<T> source, T item)
        {
            Require.NotNull(source, "source");

            if (item != null)
            {
                source.Push(item);
            }
        }
    }
}
