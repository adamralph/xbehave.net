// <copyright file="Require.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;

    internal static class Require
    {
        public static void NotNull<T>(T arg, string parameterName) where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
