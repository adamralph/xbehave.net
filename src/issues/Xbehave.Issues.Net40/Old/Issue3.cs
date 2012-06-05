// <copyright file="Issue3.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using FluentAssertions;

    using Xbehave;
    using Xbehave.Issues;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/3/new-given-overloads-for-context-disposal
    /// </summary>
    public static class Issue3
    {
        // 2 failures with 1 x 1 disposal
        [Specification]
        public static void SingleDisposable()
        {
            var disposable = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ImplicitDisposable())
                .When(() => disposable.Use());

            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 2 x 1 disposal
        [Specification]
        public static void SingleDisposableInIsolation()
        {
            var disposable = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ImplicitDisposable())
                .When(() => disposable.Use());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
        }

        // 4 failures with 3 x 1 disposal
        [Specification]
        public static void SingleDisposableMixed()
        {
            var disposable = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ImplicitDisposable())
                .When(() => disposable.Use());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 2 disposals
        [Specification]
        public static void MultipleWithActionDisposables()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);

            "Given disposables,"
                .Given(
                    () =>
                    {
                        disposable0 = new ImplicitDisposable().WithTeardown(() => disposable0.Dispose());
                        disposable1 = new ImplicitDisposable().WithTeardown(() => disposable1.Dispose());
                    })
                .When(() => disposable0.Use());

            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 2 x 2 disposals
        [Specification]
        public static void MultipleDisposablesWithActionInIsolation()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);

            "Given disposables,"
                .Given(() =>
                    {
                        disposable0 = new ImplicitDisposable().WithTeardown(() => disposable0.Dispose());
                        disposable1 = new ImplicitDisposable().WithTeardown(() => disposable1.Dispose());
                    })
                .When(() => disposable0.Use());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
        }

        // 4 failures with 3 x 2 disposals
        [Specification]
        public static void MultipleDisposablesWithActionMixed()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);

            "Given disposables,"
                .Given(() =>
                    {
                        disposable0 = new ImplicitDisposable().WithTeardown(() => disposable0.Dispose());
                        disposable1 = new ImplicitDisposable().WithTeardown(() => disposable1.Dispose());
                    })
                .When(() => disposable0.Use());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 1 disposal
        [Specification]
        public static void SingleExplicitDisposable()
        {
            var disposable = default(ExplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable = new ExplicitDisposable().WithDisposal())
                .When(() => disposable.Use());

            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }
    }
}
