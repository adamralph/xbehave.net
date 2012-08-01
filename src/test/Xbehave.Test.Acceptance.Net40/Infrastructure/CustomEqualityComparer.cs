// <copyright file="CustomEqualityComparer.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;

    internal class CustomEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> equals;
        private Func<T, int> getHashCode;

        public CustomEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            this.equals = equals;
            this.getHashCode = getHashCode;
        }

        public bool Equals(T x, T y)
        {
            return this.equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return this.getHashCode(obj);
        }
    }
}
