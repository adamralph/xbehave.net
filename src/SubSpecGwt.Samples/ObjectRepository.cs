// <copyright file="ObjectRepository.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec.Samples
{
    /// <summary>
    /// A sample specification target.
    /// </summary>
    public static class ObjectRepository
    {
        public static int Count
        {
            get { return -1; }
        }

        public static void Add<T>(T item)
        {
        }

        public static void Clear()
        {
        }
    }
}
