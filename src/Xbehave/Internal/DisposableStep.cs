// <copyright file="DisposableStep.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;

    internal class DisposableStep : StepBase<Func<IDisposable>>
    {
        public DisposableStep(string message, Func<IDisposable> action)
            : base(message, action)
        {
        }

        public IDisposable Execute()
        {
            if (this.MillisecondsTimeout > 0)
            {
                var result = this.Action.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(this.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                }
                
                return this.Action.EndInvoke(result);
            }
            
            return this.Action();
        }
    }
}
