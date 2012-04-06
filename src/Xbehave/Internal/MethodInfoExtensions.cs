// <copyright file="MethodInfoExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using Xunit.Sdk;

    internal static class MethodInfoExtensions
    {
        public static void Invoke(this IMethodInfo method)
        {
            method.Invoke(method.CreateInstanceOrDefault());
        }

        public static object CreateInstanceOrDefault(this IMethodInfo method)
        {
            return method.IsStatic ? null : method.CreateInstance();
        }
    }
}
