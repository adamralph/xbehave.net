// <copyright file="DisposableExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    internal static class DisposableExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Emulating nested using blocks.")]
        public static void DisposeAll(this IEnumerable<IDisposable> source)
        {
            Require.NotNull(source, "source");

            Exception lastEx = null;
            foreach (var disposable in source.Where(disposable => disposable != null))
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                }
            }

            if (lastEx != null)
            {
                Xunit.Sdk.ExceptionUtility.RethrowWithNoStackTraceLoss(lastEx);
            }
        }
    }
}
