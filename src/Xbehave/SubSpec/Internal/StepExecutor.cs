// <copyright file="Core.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    internal static class StepExecutor
    {
        public static void Execute(Step<Action> primitive)
        {
            if (primitive.MillisecondsTimeout > 0)
            {
                var result = primitive.Action.BeginInvoke(null, null);
                if (!result.AsyncWaitHandle.WaitOne(primitive.MillisecondsTimeout))
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
                if (!result.AsyncWaitHandle.WaitOne(primitive.MillisecondsTimeout))
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
