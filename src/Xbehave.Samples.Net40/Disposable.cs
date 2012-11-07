// <copyright file="Disposable.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System;

    public sealed class Disposable : IDisposable
    {
        public Disposable()
        {
            Console.WriteLine("CREATED: {0}", this.GetHashCode());
        }

        public void Use()
        {
            Console.WriteLine("USED: {0}", this.GetHashCode());
        }

        public void Dispose()
        {
            Console.WriteLine("DISPOSED: {0}", this.GetHashCode());
        }
    }
}
