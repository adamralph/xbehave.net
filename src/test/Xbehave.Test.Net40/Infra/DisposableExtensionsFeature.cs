// <copyright file="DisposableExtensionsFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infra
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Xbehave.Infra;
    using Xunit;

    public static class DisposableExtensionsFeature
    {
        [Scenario]
        public static void DisposingManyDisposables()
        {
            var firstDisposable = default(IDisposable);
            var secondDisposable = default(IDisposable);

            "Given a disposable"
                .Given(() => firstDisposable = A.Fake<IDisposable>(), () => { });

            "And another disposable"
                .And(() => secondDisposable = A.Fake<IDisposable>(), () => { });

            "When disposing all the disposables"
                .When(() => new[] { firstDisposable, secondDisposable }.DisposeAll());

            "Then the first disposable is disposed"
                .Then(() => A.CallTo(() => firstDisposable.Dispose()).MustHaveHappened(Repeated.Exactly.Once));

            "And the second disposable is disposed"
                .And(() => A.CallTo(() => secondDisposable.Dispose()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Scenario]
        public static void DisposingManyBadDisposables()
        {
            var firstException = default(Exception);
            var firstDisposable = default(IDisposable);
            var secondException = default(Exception);
            var secondDisposable = default(IDisposable);
            var thrownException = default(Exception);

            "Given an exception"
                .Given(() => firstException = new Exception("Exception 1"));

            "And a disposable which throws the exception"
                .And(() =>
                {
                    firstDisposable = A.Fake<IDisposable>();
                    A.CallTo(() => firstDisposable.Dispose()).Throws(firstException);
                });

            "And another exception"
                .And(() => secondException = new Exception("Exception 2"));

            "And another disposable which throws the second exception"
                .And(() =>
                {
                    secondDisposable = A.Fake<IDisposable>();
                    A.CallTo(() => secondDisposable.Dispose()).Throws(secondException);
                });

            "When disposing all the disposables"
                .When(() => thrownException = Record.Exception(() => new[] { firstDisposable, secondDisposable }.DisposeAll()));

            "Then the thrown exception should be the second exception"
                .Then(() => thrownException.Should().Be(secondException));
        }
    }
}
