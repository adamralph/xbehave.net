// <copyright file="MultipleGivensAndWhens.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System;
    using FluentAssertions;

    public class MultipleGivensAndWhens
    {
        [Scenario]
        public static void MultipleWithActionDisposables()
        {
            var disposable0 = default(Disposable);
            var disposable1 = default(Disposable);

            "Given a disposable,"
                .Given(() => disposable0 = new Disposable().WithTeardown(() => disposable0.Dispose()));

            "and another disposable"
                .And(() => disposable1 = new Disposable().WithTeardown(() => disposable1.Dispose()));

            "when using the first disposable"
                .When(() => disposable0.Use());

            "and using the second disposable"
                .And(() => disposable1.Use());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
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
