// <copyright file="Disposer.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Disposer : IDisposer
    {
        public void Dispose(IEnumerable<IDisposable> disposables)
        {
            Exception exception = null;
            foreach (var disposable in disposables.Where(x => x != null))
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            if (exception != null)
            {
                Xunit.Sdk.ExceptionUtility.RethrowWithNoStackTraceLoss(exception);
            }
        }
    }
}