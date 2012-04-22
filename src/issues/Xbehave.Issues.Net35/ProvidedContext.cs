// <copyright file="ProvidedContext.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>
#if NET40

namespace Xbehave.Issues
{
    using System;
    using FluentAssertions;

    public class ProvidedContext
    {
        [Scenario]
        public static void Disposables()
        {
            "Given a disposable,"
                .Given(x => x.Disposable0 = new Disposable(), x => x.Disposable0.Dispose());

            "and another disposable"
                .Given(x => x.Disposable1 = new Disposable(), x => x.Disposable1.Dispose());

            "when using the first disposable"
                .When(x => x.Disposable0.Use());

            "and using the second disposable"
                .When(x => x.Disposable1.Use());

            "then foo"
                .ThenInIsolation(x => ((Disposable)x.Disposable0).Should().BeNull());

            "then bar"
                .ThenInIsolation(x => ((Disposable)x.Disposable1).Should().BeNull());
        }

        public sealed class Disposable : IDisposable
        {
            public Disposable()
            {
                Console.WriteLine("CREATED");
            }

            public void Use()
            {
                Console.WriteLine("USED");
            }

            public void Dispose()
            {
                Console.WriteLine("DISPOSED");
            }
        }
    }
}
#endif
