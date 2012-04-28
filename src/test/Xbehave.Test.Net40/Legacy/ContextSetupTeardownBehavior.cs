// <copyright file="ContextSetupTeardownBehavior.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System;
    using System.Collections.Generic;
    using Xbehave;
    using Xunit;
    using Xunit.Sdk;

    public static class ContextSetupTeardownBehavior
    {
        [Specification]
        public static void MultipleAssertionsShouldCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();
            sut.ThrowInvalidOperationExceptionOnSecondCall();

            "Given an externally managed context"
                .Context(() => { });

            "when we execute an action on it that may be invoked only once"
                .Do(() =>
                    Assert.Throws<InvalidOperationException>(() => sut.ThrowInvalidOperationExceptionOnSecondCall()));

            "we expect our first assertion to pass"
                .Assert(() =>
                    Assert.True(true));

            "we expect the action not to be repeated for the second assertion"
                .Assert(() =>
                    Assert.True(true));
        }

        [Specification]
        public static void MultipleAssertionsShouldCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();
            sut.ThrowInvalidOperationExceptionOnSecondCall();

            "Given a context that may not be established twice".Context(
                () => Assert.Throws<InvalidOperationException>(() => sut.ThrowInvalidOperationExceptionOnSecondCall()));

            "when".Do(() => { });

            "we expect our first assertion to pass".Assert(() => Assert.True(true));
            "we expect the context instantiation not to be repeated for the second assertion".Assert(() => Assert.True(true));
        }

        [Specification]
        public static void MultipleObservationsShouldNotCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given an externally managed context".Context(() => { });
            "when we execute an action on it that may be invoked only once".Do(sut.ThrowInvalidOperationExceptionOnSecondCall);

            "we expect our first assertion to pass".Observation(() => Assert.True(true));
            "we expect the action not to be repeated for the second assertion".Observation(() => Assert.True(true));
        }

        [Specification]
        public static void MultipleObservationsShouldNotCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given a context that may not be established twice".Context(() => sut.ThrowInvalidOperationExceptionOnSecondCall());

            "we expect our first assertion to pass".Observation(() => Assert.True(true));
            "we expect the context instantiation not to be repeated for the second assertion".Observation(() => Assert.True(true));
        }

        public static void SpecificationThatShouldDisposeItsAssertionFixture()
        {
            "Given a disposable Fixture".ContextFixture(() => new DisposeSpy());
            "when we encounter an exception during test execution ".Do(() => { throw new Exception(); });
            "with Assertions, we expect the context is nonetheless disposed".Assert(() => { });
        }

        public static void SpecificationThatShouldDisposeItsObservationFixture()
        {
            "Given a disposable Fixture".ContextFixture(() => new DisposeSpy());
            "when we encounter an exception during test execution ".Do(() => { throw new Exception(); });
            "with Observation, we expect the context is nonetheless disposed".Observation(() => { });
        }

        private class ContextFixtureSpy
        {
            private bool called;

            public void ThrowInvalidOperationExceptionOnSecondCall()
            {
                if (this.called)
                {
                    throw new InvalidOperationException("Called twice!");
                }

                this.called = true;
            }
        }

        private class DisposeSpy : IDisposable
        {
            public static bool WasDisposed { get; private set; }

            public static void Reset()
            {
                WasDisposed = false;
            }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }
    }
}