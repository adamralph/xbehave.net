// <copyright file="StepExecutor.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Legacy
{
    using System;

    internal static class StepExecutor
    {
        public static void Execute(Step<Action> primitive)
        {
            if (primitive.MillisecondsTimeout > 0)
            {
                var result = primitive.Action.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(primitive.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(primitive.MillisecondsTimeout);
                }
                else
                {
                    primitive.Action.EndInvoke(result);
                }
            }
            else
            {
                primitive.Action();
            }
        }

        public static IDisposable Execute(Step<ContextDelegate> primitive)
        {
            if (primitive.MillisecondsTimeout > 0)
            {
                var result = primitive.Action.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(primitive.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(primitive.MillisecondsTimeout);
                }
                else
                {
                    return primitive.Action.EndInvoke(result);
                }
            }
            else
            {
                return primitive.Action();
            }
        }
    }
}
