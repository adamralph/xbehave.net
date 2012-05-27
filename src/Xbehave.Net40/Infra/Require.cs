// <copyright file="Require.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    internal static class Require
    {
        [DebuggerStepThrough]
        public static void NotNull<T>([ValidatedNotNull]T argument, string parameterName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [DebuggerStepThrough]
        public static void NotNull<T>([ValidatedNotNull]T argumentProperty, string parameterName, string propertyName) where T : class
        {
            if (argumentProperty == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0}.{1} is null.", parameterName, propertyName));
            }
        }

        // NOTE: when applied to a parameter, this attribute provides an indication to code analysis that the argument has been null checked
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
