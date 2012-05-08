// <copyright file="Disposer.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    internal class Disposer : IDisposer
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Emulating behaviour of nested using blocks")]
        public void Dispose(IEnumerable<IDisposable> disposables)
        {
            Exception lastException = null;
            foreach (var disposable in disposables.Where(disposable => disposable != null))
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            if (lastException != null)
            {
                Xunit.Sdk.ExceptionUtility.RethrowWithNoStackTraceLoss(lastException);
            }
        }
    }
}