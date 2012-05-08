// <copyright file="Disposable.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System;
    using System.Collections.Generic;

    internal class Disposable : IDisposable
    {
        private readonly Action dispose;
        private readonly IEnumerable<IDisposable> disposables;

        public Disposable(Action dispose)
        {
            this.dispose = dispose;
        }

        public Disposable(IEnumerable<IDisposable> disposables)
        {
            this.disposables = disposables;
        }

        public void Dispose()
        {
            if (this.dispose != null)
            {
                this.dispose();
            }

            if (this.disposables != null)
            {
                new Disposer().Dispose(this.disposables);
            }
        }
    }
}