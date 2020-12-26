// UPSTREAM: https://raw.githubusercontent.com/xunit/xunit/2.4.1/src/common/ExceptionExtensions.cs
#pragma warning disable IDE0011 // Add braces
#pragma warning disable IDE0019 // Use pattern matching
#pragma warning disable IDE0022 // Use expression body for methods
#pragma warning disable IDE0040 // Add accessibility modifiers
using System;
using System.Reflection;

static class ExceptionExtensions
{
#if NET35
    const string RETHROW_MARKER = "$$RethrowMarker$$";
#endif

    /// <summary>
    /// Rethrows an exception object without losing the existing stack trace information
    /// </summary>
    /// <param name="ex">The exception to re-throw.</param>
    /// <remarks>
    /// For more information on this technique, see
    /// http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2004/03/03/8353.aspx.
    /// The remote_stack_trace string is here to support Mono.
    /// </remarks>
    public static void RethrowWithNoStackTraceLoss(this Exception ex)
    {
#if NET35
        FieldInfo remoteStackTraceString =
            typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic) ??
            typeof(Exception).GetField("remote_stack_trace", BindingFlags.Instance | BindingFlags.NonPublic);

        remoteStackTraceString.SetValue(ex, ex.StackTrace + RETHROW_MARKER);
        throw ex;
#else
        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
#endif
    }

    /// <summary>
    /// Unwraps an exception to remove any wrappers, like <see cref="TargetInvocationException"/>.
    /// </summary>
    /// <param name="ex">The exception to unwrap.</param>
    /// <returns>The unwrapped exception.</returns>
    public static Exception Unwrap(this Exception ex)
    {
        while (true)
        {
            var tiex = ex as TargetInvocationException;
            if (tiex == null)
                return ex;

            ex = tiex.InnerException;
        }
    }
}
