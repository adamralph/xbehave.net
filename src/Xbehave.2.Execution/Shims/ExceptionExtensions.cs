// <copyright file="ExceptionExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Shims
{
    using System;
    using System.Reflection;
    using System.Runtime.ExceptionServices;

    internal static class ExceptionExtensions
    {
        public static void RethrowWithNoStackTraceLoss(this Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }

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