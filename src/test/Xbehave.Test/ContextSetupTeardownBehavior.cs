// <copyright file="ContextSetupTeardownBehavior.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test
{
    using System;
    using Xbehave;
    using Xbehave.Test;
    using Xunit;
    using Xunit.Sdk;

    public class ContextSetupTeardownBehavior
    {
        public static void SpecificationThatShouldDisposeItsAssertionFixture()
        {
            "Given a disposable Fixture".Given(() => new DisposeSpy());
            "when we encounter an exception during test execution ".When(() => { throw new Exception(); });
            "with Assertions, we expect the context is nonetheless disposed".ThenInIsolation(() => { });
        }

        public static void SpecificationThatShouldDisposeItsObservationFixture()
        {
            "Given a disposable Fixture".Given(() => new DisposeSpy());
            "when we encounter an exception during test execution ".When(() => { throw new Exception(); });
            "with Observation, we expect the context is nonetheless disposed".Then(() => { });
        }

        [Fact]
        public void SutThrowsWhenCalledTwice()
        {
            var sut = new ContextFixtureSpy();
            Assert.Throws<InvalidOperationException>(() =>
            {
                sut.FailWhenCallingTwice();
                sut.FailWhenCallingTwice();
            });
        }

        [Scenario]
        public void MultipleAssertionsShouldCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();
            sut.FailWhenCallingTwice();

            "Given an externally managed context"
                .Given(() => { });

            "when we execute an action on it that may be invoked only once"
                .When(() =>
                    Assert.Throws<InvalidOperationException>(() => sut.FailWhenCallingTwice()));

            "we expect our first assertion to pass"
                .ThenInIsolation(() =>
                    Assert.True(true));

            "we expect the action not to be repeated for the second assertion"
                .ThenInIsolation(() =>
                    Assert.True(true));
        }

        [Scenario]
        public void MultipleAssertionsShouldCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();
            sut.FailWhenCallingTwice();

            "Given a context that may not be established twice".Given(
                () => Assert.Throws<InvalidOperationException>(() => sut.FailWhenCallingTwice()));

            "when nothing happens".When(() => { });

            "we expect our first assertion to pass".ThenInIsolation(() => { Assert.True(true); });
            "we expect the context instantiation not to be repeated for the second assertion".ThenInIsolation(() => { Assert.True(true); });
        }

        [Scenario]
        public void MultipleObservationsShouldNotCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given an externally managed context".Given(() => { });
            "when we execute an action on it that may be invoked only once".When(() => { sut.FailWhenCallingTwice(); });

            "we expect our first assertion to pass".Then(() => { Assert.True(true); });
            "we expect the action not to be repeated for the second assertion".Then(() => { Assert.True(true); });
        }

        [Scenario]
        public void MultipleObservationsShouldNotCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given a context that may not be established twice".Given(() => { sut.FailWhenCallingTwice(); });

            "we expect our first assertion to pass".Then(() => { Assert.True(true); });
            "we expect the context instantiation not to be repeated for the second assertion".Then(() => { Assert.True(true); });
        }

        [Fact]
        public void ErrorInDoForAssertionDisposesContext()
        {
            DisposeSpy.Reset();

            IMethodInfo method = Reflector.Wrap(StaticReflection.MethodOf(() => SpecificationThatShouldDisposeItsAssertionFixture()));

            ExecuteSpecification(method);

            Assert.True(DisposeSpy.WasDisposed);
        }

        [Fact]
        public void ErrorInDoForObservationDisposesContext()
        {
            DisposeSpy.Reset();

            IMethodInfo method = Reflector.Wrap(StaticReflection.MethodOf(() => SpecificationThatShouldDisposeItsObservationFixture()));
            ExecuteSpecification(method);

            Assert.True(DisposeSpy.WasDisposed);
        }

        private static void ExecuteSpecification(IMethodInfo method)
        {
            foreach (var command in ScenarioAttribute.GetFactCommands(method))
            {
                command.Execute(null);
            }
        }

        private class ContextFixtureSpy
        {
            private bool called = false;

            public void FailWhenCallingTwice()
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
            private static bool disposed = false;

            public static bool WasDisposed
            {
                get { return disposed; }
            }

            public static void Reset()
            {
                disposed = false;
            }

            public void Dispose()
            {
                disposed = true;
            }
        }
    }
}