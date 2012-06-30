// <copyright file="Covariance.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples.Net35
{
    using System;

    using FluentAssertions;

    public class Covariance
    {
        [Scenario]
        public static void WorkaroundLackOfCovariance()
        {
            var disposable = default(ImplicitDisposable);
            Func<ImplicitDisposable> func = () => disposable = new ImplicitDisposable();

            "Given a disposable,"
                .Given(() => func())
                .When(() => disposable.ToString())
                .Then(() => true.Should().Be(false)).InIsolation();
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
