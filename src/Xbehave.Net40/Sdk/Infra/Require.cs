// <copyright file="Require.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System;
    using System.Diagnostics;

    internal static class Require
    {
        [DebuggerStepThrough]
        public static void NotNull<T>([ValidatedNotNull]T arg, string parameterName) where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        // NOTE: when applied to a parameter, this attribute provides an indication to code analysis that the argument has been null checked
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
