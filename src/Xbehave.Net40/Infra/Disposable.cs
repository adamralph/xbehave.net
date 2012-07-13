// <copyright file="Disposable.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Infra
{
    using System;

    internal class Disposable : IDisposable
    {
        private readonly Action disposal;

        public Disposable(Action disposal)
        {
            this.disposal = disposal;
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