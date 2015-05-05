// <copyright file="ExceptionExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Extensions
{
    using System;
    using System.Reflection;

    internal static class ExceptionExtensions
    {
        public static Exception Unwrap(this Exception ex)
        {
            while (true)
            {
                var tiex = ex as TargetInvocationException;
                if (tiex == null)
                {
                    return ex;
                }

                ex = tiex.InnerException;
            }
        }
    }
}
