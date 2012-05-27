// <copyright file="Disposable.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System;
    using System.Collections.Generic;

    internal class Disposable : IDisposable
    {
        private readonly Action disposal;

        public Disposable(Action disposal)
        {
            this.disposal = disposal;
        }

        public Disposable(IEnumerable<IDisposable> disposables)
        {
            if (disposables != null)
            {
                this.disposal = () => disposables.DisposeAll();
            }
        }

        public void Dispose()
        {
            if (this.disposal != null)
            {
                this.disposal.Invoke();
            }
        }
    }
}