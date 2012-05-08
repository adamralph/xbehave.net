// <copyright file="StackExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class StackExtensions
    {
        // NOTE: in practice, we could just pass Stack as IEnumerable since it enumerates in reverse order as required, but this behaviour is undocumented
        public static IEnumerable<T> Unwind<T>(this Stack<T> source)
        {
            while (source.Any())
            {
                yield return source.Pop();
            }
        }
    }
}
