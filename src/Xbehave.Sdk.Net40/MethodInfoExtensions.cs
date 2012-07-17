// <copyright file="MethodInfoExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using Xunit.Sdk;

    public static class MethodInfoExtensions
    {
        public static int GetTimeoutParameter(this IMethodInfo method)
        {
            if (method == null)
            {
                return -1;
            }

            return MethodUtility.GetTimeoutParameter(method);
        }
    }
}
