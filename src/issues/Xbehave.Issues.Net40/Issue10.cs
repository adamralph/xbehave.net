// <copyright file="Issue10.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System;
    using FluentAssertions;
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/10
    /// </summary>
    public class Issue10
    {
        [Scenario]
        public static void Usings()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);

            "Given a disposable,"
                .Given((Action)(() => disposable0 = new ImplicitDisposable().Using()));

            "and another disposable"
                .And((Action)(() => disposable1 = new ImplicitDisposable().Using()));

            "when using the first disposable"
                .When(() => disposable0.Use());

            "and using the second disposable"
                .And(() => disposable1.Use());

            _.Then(() => true.Should().Be(false)).InIsolation();
            _.Then(() => true.Should().Be(false)).InIsolation();
        }

        [Scenario]
        public static void Teardowns()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);
            var disposable2 = default(ImplicitDisposable);
            var disposable3 = default(ImplicitDisposable);

            "Given a disposable,"
                .Given((Action)(() => disposable0 = new ImplicitDisposable()))
                .Teardown(() => disposable0.Dispose());

            "and another disposable"
                .And((Action)(() => disposable1 = new ImplicitDisposable()))
                .Teardown(() => disposable1.Dispose());

            "and some more disposables"
                .And(() =>
                {
                    disposable2 = new ImplicitDisposable();
                    disposable3 = new ImplicitDisposable();
                })
                .Teardown(() => disposable2.Dispose())
                .And()
                .Teardown(() => disposable3.Dispose());

            "when using the first disposable"
                .When(() => disposable0.Use());

            "and using the second disposable"
                .And(() => disposable1.Use());

            _.Then(() => true.Should().Be(false)).InIsolation();
            _.Then(() => true.Should().Be(false)).InIsolation();
        }

        [Scenario]
        public static void UsingsInTeardowns()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);
            var disposable2 = default(ImplicitDisposable);
            var disposable3 = default(ImplicitDisposable);

            "Given a disposable,"
                .Given((Action)(() => disposable0 = new ImplicitDisposable()))
                .Teardown(() => disposable0.Using());

            "and another disposable"
                .And((Action)(() => disposable1 = new ImplicitDisposable()))
                .Teardown(() => disposable1.Using());

            "and some more disposables"
                .And(() =>
                {
                    disposable2 = new ImplicitDisposable();
                    disposable3 = new ImplicitDisposable();
                })
                .Teardown(() => disposable2.Using())
                .And()
                .Teardown(() => disposable3.Using())
                .And()
                .WithTimeout(100000);

            "when using the first disposable"
                .When(() => disposable0.Use());

            "and using the second disposable"
                .And(() => disposable1.Use());

            _.Then(() => true.Should().Be(false)).InIsolation();
            _.Then(() => true.Should().Be(false)).InIsolation();
        }
    }
}
