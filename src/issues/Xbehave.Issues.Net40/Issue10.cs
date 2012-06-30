// <copyright file="Issue10.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using FluentAssertions;
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/10
    /// </summary>
    public class Issue10
    {
        [Scenario]
        public static void MultipleDisposables()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable0 = new ImplicitDisposable(), () => disposable0.Dispose());

            "and another disposable"
                .And(() => disposable1 = new ImplicitDisposable(), () => disposable1.Dispose());

            "when using the first disposable"
                .When(() => disposable0.Use());

            "and using the second disposable"
                .And(() => disposable1.Use());

            _.Then(() => true.Should().Be(false)).InIsolation();
            _.Then(() => true.Should().Be(false)).InIsolation();
        }

        [Scenario]
        public static void RegisteringDisposableInTeardown()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() => disposable0 = new ImplicitDisposable(), () => disposable0.Using());

            "and another disposable"
                .And(() => disposable1 = new ImplicitDisposable(), () => disposable1.Using());

            "when using the first disposable"
                .When(() => disposable0.Use());

            "and using the second disposable"
                .And(() => disposable1.Use());

            _.Then(() => true.Should().Be(false)).InIsolation();
            _.Then(() => true.Should().Be(false)).InIsolation();
        }
    }
}
