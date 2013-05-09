// <copyright file="ContextSetupTeardownBehavior.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Legacy
{
    using System;
    using Xbehave;
    using Xunit;

    public class ContextSetupTeardownBehavior
    {
        [Scenario]
        public static void MultipleAssertionsShouldCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();
            sut.ThrowInvalidOperationExceptionOnSecondCall();

            "Given an externally managed context"
                .Given(() => { });

            "when we execute an action on it that may be invoked only once"
                .When(() => Assert.Throws<InvalidOperationException>(() => sut.ThrowInvalidOperationExceptionOnSecondCall()));

            "we expect our first assertion to pass"
                .Then(() => Assert.True(true))
                .InIsolation();

            "we expect the action not to be repeated for the second assertion"
                .Then(() => Assert.True(true))
                .InIsolation();
        }

        [Scenario]
        public static void MultipleAssertionsShouldCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();
            sut.ThrowInvalidOperationExceptionOnSecondCall();

            "Given a context that may not be established twice"
                .Given(() => Assert.Throws<InvalidOperationException>(() => sut.ThrowInvalidOperationExceptionOnSecondCall()));

            "when"
                .When(() => { });

            "we expect our first assertion to pass"
                .Then(() => Assert.True(true))
                .InIsolation();
            
            "we expect the context instantiation not to be repeated for the second assertion"
                .Then(() => Assert.True(true))
                .InIsolation();
        }

        [Scenario]
        public static void MultipleObservationsShouldNotCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given an externally managed context".Given(() => { });
            "when we execute an action on it that may be invoked only once".When(sut.ThrowInvalidOperationExceptionOnSecondCall);

            "we expect our first assertion to pass".Then(() => Assert.True(true));
            "we expect the action not to be repeated for the second assertion".Then(() => Assert.True(true));
        }

        [Scenario]
        public static void MultipleObservationsShouldNotCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given a context that may not be established twice".Given(() => sut.ThrowInvalidOperationExceptionOnSecondCall());

            "we expect our first assertion to pass".Then(() => Assert.True(true));
            "we expect the context instantiation not to be repeated for the second assertion".Then(() => Assert.True(true));
        }

        public static void SpecificationThatShouldDisposeItsAssertionFixture()
        {
            "Given a disposable Fixture".Given(() => new DisposeSpy());
            "when we encounter an exception during test execution ".When(() => { throw new Exception(); });
            "with Assertions, we expect the context is nonetheless disposed".Then(() => { });
        }

        public static void SpecificationThatShouldDisposeItsObservationFixture()
        {
            "Given a disposable Fixture".Given(() => new DisposeSpy());
            "when we encounter an exception during test execution ".When(() => { throw new Exception(); });
            "with Observation, we expect the context is nonetheless disposed".Then(() => { });
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