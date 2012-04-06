// <copyright file="StepExecutor.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;

    internal static class StepExecutor
    {
        public static void Execute(Step<Action> step)
        {
            if (step.MillisecondsTimeout > 0)
            {
                var result = step.Action.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(step.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(step.MillisecondsTimeout);
                }
                else
                {
                    step.Action.EndInvoke(result);
                }
            }
            else
            {
                step.Action();
            }
        }

        public static IDisposable Execute(Step<Func<IDisposable>> step)
        {
            if (step.MillisecondsTimeout > 0)
            {
                var result = step.Action.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(step.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(step.MillisecondsTimeout);
                }
                else
                {
                    return step.Action.EndInvoke(result);
                }
            }
            else
            {
                return step.Action();
            }
        }
    }
}
