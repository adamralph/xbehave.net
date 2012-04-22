// <copyright file="MethodInfoExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Naming
{
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class MethodInfoExtensions
    {
        public static bool IsExtension(this MethodInfo method)
        {
            return method.IsDefined(typeof(ExtensionAttribute), false);
        }
    }
}
