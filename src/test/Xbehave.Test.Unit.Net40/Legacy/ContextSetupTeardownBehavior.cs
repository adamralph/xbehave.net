// <copyright file="ContextSetupTeardownBehavior.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Legacy
{
    using System;
    using Xbehave;
    using Xunit;

    public static class ContextSetupTeardownBehavior
    {
        [Scenario]
        public static void MultipleContextsShouldCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "when we execute an action"
                .When(sut.Call);

            "we expect the action to be called in the first context"
                .Then(() => Assert.Equal(1, sut.Called))
                .InIsolation();

            "we expect the action to be repeated for the second context"
                .Then(() => Assert.Equal(2, sut.Called))
                .InIsolation();
        }

        [Scenario]
        public static void MultipleContextsShouldCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given a context instantiation"
                .Given(sut.Call);

            "we expect the context instantiation to be called in the first context"
                .Then(() => Assert.Equal(1, sut.Called))
                .InIsolation();

            "we expect the context instantiation to be repeated for the second context"
                .Then(() => Assert.Equal(2, sut.Called))
                .InIsolation();
        }

        [Scenario]
        public static void MultipleAssertionsShouldNotCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "when we execute an action".When(sut.Call);
            "... that may not be called twice".And(sut.ThrowIfCalledTwice);

            "we expect the action to be called once".Then(() => Assert.Equal(1, sut.Called));
            "we expect the action not to be repeated for the second assertion".Then(() => Assert.Equal(1, sut.Called));
        }

        [Scenario]
        public static void MultipleAssertionsShouldNotCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given a context instantiation".Given(sut.Call);
            "... that may not be called twice".And(sut.ThrowIfCalledTwice);

            "we expect the context instantiation to be called once".Then(() => Assert.Equal(1, sut.Called));
            "we expect the context instantiation not to be repeated for the second assertion".Then(() => Assert.Equal(1, sut.Called));
        }

        private class ContextFixtureSpy
        {
            public int Called { get; private set; }

            public void Call()
            {
                this.Called++;
            }

            public void ThrowIfCalledTwice()
            {
                if (this.Called > 1)
                {
                    throw new InvalidOperationException("Called twice!");
                }
            }
        }
    }
}