// <copyright file="EnumerableEqualityComparer.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>> where T : IEquatable<T>
    {
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            if (x != null)
            {
                return y != null && x.SequenceEqual(y);
            }

            return y == null;
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            unchecked
            {
                return obj == null ? 0 : obj.Aggregate(17, (hash, item) => (hash * 23) + EqualityComparer<T>.Default.GetHashCode(item));
            }
        }
    }
}
