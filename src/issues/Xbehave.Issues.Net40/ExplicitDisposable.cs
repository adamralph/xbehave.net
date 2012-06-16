// <copyright file="ExplicitDisposable.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class ExplicitDisposable : IDisposable
    {
        public ExplicitDisposable()
        {
            Console.WriteLine("CREATED: {0}", this.GetHashCode());
        }

        public void Use()
        {
            Console.WriteLine("USED: {0}", this.GetHashCode());
        }

        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Required to be an explicit implementation.")]
        void IDisposable.Dispose()
        {
            Console.WriteLine("DISPOSED: {0}", this.GetHashCode());
        }
    }
}
