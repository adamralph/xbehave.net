// <copyright file="Guard.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    internal static class Guard
    {
        [DebuggerStepThrough]
        public static void AgainstNullArgument<T>(string parameterName, [ValidatedNotNull]T argument) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, string.Format(CultureInfo.InvariantCulture, "{0} is null.", parameterName));
            }
        }

        [DebuggerStepThrough]
        public static void AgainstNullArgumentProperty<T>(string parameterName, string propertyName, [ValidatedNotNull]T argumentProperty) where T : class
        {
            if (argumentProperty == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0}.{1} is null.", parameterName, propertyName), parameterName);
            }
        }

        // NOTE: when applied to a parameter, this attribute provides an indication to code analysis that the argument has been null checked
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
