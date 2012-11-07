// <copyright file="Optimizations.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System;
    using System.Collections.Generic;
    using Guard = Xbehave.Sdk.Guard;

    public static class Optimizations
    {
        private static readonly HashSet<string> IgnoredTypes = new HashSet<string>
        { 
            "FakeItEasy.Repeated",
        };

        public static bool IsIgnored(this Type type)
        {
            Guard.AgainstNullArgument("type", type);

            return type.FullName != null && IgnoredTypes.Contains(type.FullName);
        }
    }
}
