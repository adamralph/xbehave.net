// <copyright file="Disposable.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System;
    using System.Collections.Generic;

    internal class Disposable : IDisposable
    {
        private readonly Action disposal;

        public Disposable(Action disposal)
        {
            Require.NotNull(disposal, "disposal");

            this.disposal = disposal;
        }

        public Disposable(IEnumerable<IDisposable> disposables)
        {
            Require.NotNull(disposables, "disposables");

            this.disposal = () => disposables.DisposeAll();
        }

        public void Dispose()
        {
            this.disposal.Invoke();
        }
    }
}