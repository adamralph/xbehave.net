// <copyright file="DisposableArrangement.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FluentAssertions;

    using Xbehave;

    public static class DisposableArrangement
    {
        // 2 failures with 1 x 1 disposal
        [Scenario]
        public static void SingleDisposable()
        {
            var disposable = default(IDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ImplicitDisposable())
                .When(() => disposable.ToString())
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        // 2 failures with 2 x 1 disposal
        [Scenario]
        public static void SingleDisposableInIsolation()
        {
            var disposable = default(IDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ImplicitDisposable())
                .When(() => disposable.ToString())
                .ThenInIsolation(() => true.Should().Be(false))
                .ThenInIsolation(() => true.Should().Be(false));
        }

        // 4 failures with 3 x 1 disposal
        [Scenario]
        public static void SingleDisposableMixed()
        {
            var disposable = default(IDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ImplicitDisposable())
                .When(() => disposable.ToString())
                .ThenInIsolation(() => true.Should().Be(false))
                .ThenInIsolation(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 2 disposals
        [Scenario]
        public static void MultipleWithEnumerableDisposables()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(() => new[] { disposable0 = new ImplicitDisposable(), disposable1 = new ImplicitDisposable() })
                .When(() => disposable0.ToString())
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        // 2 failures with 2 x 2 disposals
        [Scenario]
        public static void MultipleEnumerableDisposablesInIsolation()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(() => new[] { disposable0 = new ImplicitDisposable(), disposable1 = new ImplicitDisposable() })
                .When(() => disposable0.ToString())
                .ThenInIsolation(() => true.Should().Be(false))
                .ThenInIsolation(() => true.Should().Be(false));
        }

        // 4 failures with 3 x 2 disposals
        [Scenario]
        public static void MultipleEnumerableDisposablesMixed()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(() => new[] { disposable0 = new ImplicitDisposable(), disposable1 = new ImplicitDisposable() })
                .When(() => disposable0.ToString())
                .ThenInIsolation(() => true.Should().Be(false))
                .ThenInIsolation(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 2 disposals
        [Scenario]
        public static void MultipleActionDisposables()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(
                    () =>
                    {
                        disposable0 = new ImplicitDisposable();
                        disposable1 = new ImplicitDisposable();
                    },
                    () =>
                    {
                        disposable0.Dispose();
                        disposable1.Dispose();
                    })
                .When(() => disposable0.ToString())
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        // 2 failures with 2 x 2 disposals
        [Scenario]
        public static void MultipleActionDisposablesInIsolation()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(
                    () =>
                    {
                        disposable0 = new ImplicitDisposable();
                        disposable1 = new ImplicitDisposable();
                    },
                    () =>
                    {
                        disposable0.Dispose();
                        disposable1.Dispose();
                    })
                .When(() => disposable0.ToString())
                .ThenInIsolation(() => true.Should().Be(false))
                .ThenInIsolation(() => true.Should().Be(false));
        }

        // 4 failures with 3 x 2 disposals
        [Scenario]
        public static void MultipleActionDisposablesMixed()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(
                    () =>
                    {
                        disposable0 = new ImplicitDisposable();
                        disposable1 = new ImplicitDisposable();
                    },
                    () =>
                    {
                        disposable0.Dispose();
                        disposable1.Dispose();
                    })
                .When(() => disposable0.ToString())
                .ThenInIsolation(() => true.Should().Be(false))
                .ThenInIsolation(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 1 disposal
        [Scenario]
        public static void SingleImplicitDisposable()
        {
            var disposable = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ImplicitDisposable())
                .When(() => disposable.ToString())
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 1 disposal
        [Scenario]
        public static void SingleExplicitDisposable()
        {
            var disposable = default(ExplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ExplicitDisposable())
                .When(() => disposable.ToString())
                .Then(() => true.Should().Be(false))
                .Then(() => true.Should().Be(false));
        }

        public sealed class ImplicitDisposable : IDisposable
        {
            public void Dispose()
            {
                Console.WriteLine("DISPOSED");
            }
        }

        public sealed class ExplicitDisposable : IDisposable
        {
            [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Required to be an explicit implementation.")]
            void IDisposable.Dispose()
            {
                Console.WriteLine("DISPOSED");
            }
        }
    }
}
