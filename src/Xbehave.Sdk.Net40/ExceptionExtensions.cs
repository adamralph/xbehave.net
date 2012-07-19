// <copyright file="ExceptionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Utility methods for dealing with exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Rethrows an exception object without losing the existing stack trace information
        /// </summary>
        /// <param name="ex">The exception to re-throw.</param>
        /// <remarks>
        /// For more information on this technique, see
        /// http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2004/03/03/8353.aspx
        /// </remarks>
        public static void RethrowWithNoStackTraceLoss(this Exception ex)
        {
            var fieldInfo = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic) ??
                typeof(Exception).GetField("remote_stack_trace", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo.SetValue(ex, ex.StackTrace + "$$RethrowMarker$$");
            throw ex;
        }
    }
}