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
        [Fact]
        public static void SutThrowsWhenCalledTwice()
        {
            var sut = new ContextFixtureSpy();
            Assert.Throws<InvalidOperationException>(() =>
            {
                sut.FailWhenCallingTwice();
                sut.FailWhenCallingTwice();
            });
        }

        [Specification]
        public static void MultipleAssertionsShouldCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();
            sut.FailWhenCallingTwice();

            "Given an externally managed context"
                .Context(() => { });

            "when we execute an action on it that may be invoked only once"
                .Do(() =>
                    Assert.Throws<InvalidOperationException>(() => sut.FailWhenCallingTwice()));

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
            sut.FailWhenCallingTwice();

            "Given a context that may not be established twice".Context(
                () => Assert.Throws<InvalidOperationException>(() => sut.FailWhenCallingTwice()));

            "when".Do(() => { });

            "we expect our first assertion to pass".Assert(() => { Assert.True(true); });
            "we expect the context instantiation not to be repeated for the second assertion".Assert(() => { Assert.True(true); });
        }

        [Specification]
        public static void MultipleObservationsShouldNotCauseActionToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given an externally managed context".Context(() => { });
            "when we execute an action on it that may be invoked only once".Do(() => { sut.FailWhenCallingTwice(); });

            "we expect our first assertion to pass".Observation(() => { Assert.True(true); });
            "we expect the action not to be repeated for the second assertion".Observation(() => { Assert.True(true); });
        }

        [Specification]
        public static void MultipleObservationsShouldNotCauseContextInstantiationToBeRepeated()
        {
            var sut = new ContextFixtureSpy();

            "Given a context that may not be established twice".Context(() => { sut.FailWhenCallingTwice(); });

            "we expect our first assertion to pass".Observation(() => { Assert.True(true); });
            "we expect the context instantiation not to be repeated for the second assertion".Observation(() => { Assert.True(true); });
        }

        public static void SpecificationThatShouldDisposeItsAssertionFixture()
        {
            "Given a disposable Fixture".ContextFixture(() => new DisposeSpy());
            "when we encounter an exception during test execution ".Do(() => { throw new Exception(); });
            "with Assertions, we expect the context is nonetheless disposed".Assert(() => { });
        }

        [Fact]
        public static void ErrorInDoForAssertionDisposesContext()
        {
            DisposeSpy.Reset();

            IMethodInfo method = Reflector.Wrap(StaticReflection.MethodOf(() => SpecificationThatShouldDisposeItsAssertionFixture()));

            ExecuteSpecification(method);

            Assert.True(DisposeSpy.WasDisposed);
        }

        public static void SpecificationThatShouldDisposeItsObservationFixture()
        {
            "Given a disposable Fixture".ContextFixture(() => new DisposeSpy());
            "when we encounter an exception during test execution ".Do(() => { throw new Exception(); });
            "with Observation, we expect the context is nonetheless disposed".Observation(() => { });
        }

        [Fact]
        public static void ErrorInDoForObservationDisposesContext()
        {
            DisposeSpy.Reset();

            IMethodInfo method = Reflector.Wrap(StaticReflection.MethodOf(() => SpecificationThatShouldDisposeItsObservationFixture()));
            ExecuteSpecification(method);

            Assert.True(DisposeSpy.WasDisposed);
        }

        private static void ExecuteSpecification(IMethodInfo spec)
        {
            foreach (var command in new TestAttribute().EnumerateTestCommands(spec))
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

        private class TestAttribute : ScenarioAttribute
        {
            public new IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
            {
                return base.EnumerateTestCommands(method);
            }
        }
    }
}