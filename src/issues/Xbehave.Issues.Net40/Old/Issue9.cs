// <copyright file="Issue9.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using System;

    using FluentAssertions;

    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/9/add-given-func
    /// </summary>
    public class Issue9
    {
        [Specification]
        public static void AutonamingWithExplicitDisposalAction()
        {
            var disposable0 = default(Disposable);
            var disposable1 = default(Disposable);
            var disposable2 = default(Disposable);

            "Given some disposables"
                .Given(
                () => new[]
                    {
                        disposable0 = new Disposable(),
                        disposable1 = new Disposable(),
                        disposable2 = new Disposable(),
                    });

            "when the disposables are used"
                .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    })
                .ThenInIsolation(() => true.Should().Be(false));
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
