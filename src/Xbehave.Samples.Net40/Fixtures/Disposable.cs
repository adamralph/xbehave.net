// <copyright file="Disposable.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples.Fixtures
{
    using System;

    public sealed class Disposable : IDisposable
    {
        private readonly string id = Guid.NewGuid().ToString();

        public Disposable()
        {
            Console.WriteLine("CREATED: {0}", this.id);
        }

        public void Use()
        {
            Console.WriteLine("USED: {0}", this.id);
        }

        public void Dispose()
        {
            Console.WriteLine("DISPOSED: {0}", this.id);
        }
    }
}
