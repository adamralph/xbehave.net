// <copyright file="ArrayComparer.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Generic;

    internal class ArrayComparer<T> : IComparer<T[]> where T : IComparable
    {
        public int Compare(T[] x, T[] y)
        {
            for (int index = 0; index < Math.Max(x.Length, y.Length); ++index)
            {
                if (index >= x.Length)
                {
                    return -1;
                }

                if (index >= y.Length)
                {
                    return 1;
                }

                var comparison = x[index].CompareTo(y[index]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return 0;
        }
    }
}
