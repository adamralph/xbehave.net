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
        // NOTE (aralph1): in practice we could accept IEnumerable since a Stack enumerates in a reverse order as required, but this behaviour is undocumented
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Emulating behaviour of nested using blocks")]
        public void Dispose(Stack<IDisposable> disposables)
        {
            Exception lastException = null;
            while (disposables.Any())
            {
                var disposable = disposables.Pop();
                if (disposable == null)
                {
                    continue;
                }

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