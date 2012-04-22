// <copyright file="Issue3.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy;

    using FluentAssertions;

    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/3/new-given-overloads-for-context-disposal
    /// </summary>
    public static class Issue3
    {
        // 2 failures with 1 x 1 disposal
        [Specification]
        public static void SingleDisposable()
        {
            var disposable = default(IDisposable);

            "Given a disposable,"
                .Given(() =>
                    {
                        disposable = A.Fake<IDisposable>();
                        A.CallTo(() => disposable.Dispose()).Invokes(x => Console.WriteLine("DISPOSED"));
                        return disposable;
                    })
                .When(() => disposable.ToString());

            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 2 x 1 disposal
        [Specification]
        public static void SingleDisposableInIsolation()
        {
            var disposable = default(IDisposable);

            "Given a disposable,"
                .Given(() =>
                    {
                        disposable = A.Fake<IDisposable>();
                        A.CallTo(() => disposable.Dispose()).Invokes(x => Console.WriteLine("DISPOSED"));
                        return disposable;
                    })
                .When(() => disposable.ToString());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
        }

        // 4 failures with 3 x 1 disposal
        [Specification]
        public static void SingleDisposableMixed()
        {
            var disposable = default(IDisposable);

            "Given a disposable,"
                .Given(() =>
                {
                    disposable = A.Fake<IDisposable>();
                    A.CallTo(() => disposable.Dispose()).Invokes(x => Console.WriteLine("DISPOSED"));
                    return disposable;
                })
                .When(() => disposable.ToString());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 2 disposals
        [Specification]
        public static void MultipleWithActionDisposables()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(
                    () =>
                    {
                        disposable0 = A.Fake<IDisposable>();
                        A.CallTo(() => disposable0.Dispose()).Invokes(x => Console.WriteLine("DISPOSED0"));

                        disposable1 = A.Fake<IDisposable>();
                        A.CallTo(() => disposable1.Dispose()).Invokes(x => Console.WriteLine("DISPOSED1"));
                    },
                    () =>
                    {
                        disposable0.Dispose();
                        disposable1.Dispose();
                    })
                .When(() => disposable0.ToString());

            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 2 x 2 disposals
        [Specification]
        public static void MultipleDisposablesWithActionInIsolation()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(
                    () =>
                    {
                        disposable0 = A.Fake<IDisposable>();
                        A.CallTo(() => disposable0.Dispose()).Invokes(x => Console.WriteLine("DISPOSED0"));

                        disposable1 = A.Fake<IDisposable>();
                        A.CallTo(() => disposable1.Dispose()).Invokes(x => Console.WriteLine("DISPOSED1"));
                    },
                    () =>
                    {
                        disposable0.Dispose();
                        disposable1.Dispose();
                    })
                .When(() => disposable0.ToString());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
        }

        // 4 failures with 3 x 2 disposals
        [Specification]
        public static void MultipleDisposablesWithActionMixed()
        {
            var disposable0 = default(IDisposable);
            var disposable1 = default(IDisposable);

            "Given disposables,"
                .Given(
                    () =>
                    {
                        disposable0 = A.Fake<IDisposable>();
                        A.CallTo(() => disposable0.Dispose()).Invokes(x => Console.WriteLine("DISPOSED0"));

                        disposable1 = A.Fake<IDisposable>();
                        A.CallTo(() => disposable1.Dispose()).Invokes(x => Console.WriteLine("DISPOSED1"));
                    },
                    () =>
                    {
                        disposable0.Dispose();
                        disposable1.Dispose();
                    })
                .When(() => disposable0.ToString());

            _.ThenInIsolation(() => true.Should().Be(false));
            _.ThenInIsolation(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 1 disposal
        [Specification]
        public static void SingleImplicittDisposable()
        {
            var disposable = default(ImplicitDisposable);

            "Given a disposable,"
                .Given(() =>
                {
                    disposable = new ImplicitDisposable();
                    return disposable;
                })
                .When(() => disposable.ToString());

            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
        }

        // 2 failures with 1 x 1 disposal
        [Specification]
        public static void SingleExplicitDisposable()
        {
            var disposable = default(ExplicitDisposable);

            "Given a disposable,"
                .Given(() =>
                {
                    disposable = new ExplicitDisposable();
                    return disposable;
                })
                .When(() => disposable.ToString());

            _.Then(() => true.Should().Be(false));
            _.Then(() => true.Should().Be(false));
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
