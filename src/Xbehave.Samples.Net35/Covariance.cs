// <copyright file="Covariance.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples.Net35
{
    using System;

    using FluentAssertions;

    public class Covariance
    {
        [Specification]
        public static void WorkaroundLackOfCovariance()
        {
            var disposable = default(ImplicitDisposable);
            Func<ImplicitDisposable> func = () => disposable = new ImplicitDisposable();

            "Given a disposable,"
                .Given(() => func())
                .When(() => disposable.ToString())
                .ThenInIsolation(() => true.Should().Be(false));
        }

        [Specification]
        public static void Inline()
        {
            var disposable = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() =>
                {
                    disposable = new ImplicitDisposable();
                    return disposable;
                })
                .When(() => disposable.ToString())
                .ThenInIsolation(() => true.Should().Be(false));
        }

        public sealed class ImplicitDisposable : IDisposable
        {
            public void Dispose()
            {
                Console.WriteLine("DISPOSED");
            }
        }
    }
}
