// <copyright file="Optimizations.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Naming
{
    using System;
    using System.Collections.Generic;

    internal static class Optimizations
    {
        private static readonly HashSet<string> ignoredTypes = new HashSet<string>
        { 
            "FakeItEasy.Repeated",
        };

        public static bool IsIgnored(this Type type)
        {
            return type.FullName != null && ignoredTypes.Contains(type.FullName);
        }
    }
}
