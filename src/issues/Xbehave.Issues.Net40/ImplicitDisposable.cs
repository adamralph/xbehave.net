// <copyright file="ImplicitDisposable.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System;

    public sealed class ImplicitDisposable : IDisposable
    {
        public ImplicitDisposable()
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
