using System;
using Xbehave;
using Xunit;
using Xunit.Sdk;
using TestUtility;
using System.Collections;

public class ContextSetupTeardownBehavior
{
    private class ContextFixtureSpy
    {
        private bool called = false;
        public void FailWhenCallingTwice()
        {
            if (called)
                throw new InvalidOperationException( "Called twice!" );

            called = true;
        }
    }

    [Fact]
    public void SutThrowsWhenCalledTwice()
    {
        var sut = new ContextFixtureSpy();
        Assert.Throws<InvalidOperationException>( () =>
        {
            sut.FailWhenCallingTwice();
            sut.FailWhenCallingTwice();
        } );
    }

    [Specification]
    public void MultipleAssertionsShouldCauseActionToBeRepeated()
    {
        var sut = new ContextFixtureSpy();
        sut.FailWhenCallingTwice();

        "Given an externally managed context"
            .Context( () => { } );

        "when we execute an action on it that may be invoked only once"
            .Do( () =>
                Assert.Throws<InvalidOperationException>( () => sut.FailWhenCallingTwice() ) );

        "we expect our first assertion to pass"
            .Assert( () =>
                Assert.True( true ) );

        "we expect the action not to be repeated for the second assertion"
            .Assert( () =>
                Assert.True( true ) );
    }

    [Specification]
    public void MultipleAssertionsShouldCauseContextInstantiationToBeRepeated()
    {
        var sut = new ContextFixtureSpy();
        sut.FailWhenCallingTwice();

        "Given a context that may not be established twice".Context(
            () => Assert.Throws<InvalidOperationException>( () => sut.FailWhenCallingTwice() ) );

        "".Do( () => { } );

        "we expect our first assertion to pass".Assert( () => { Assert.True( true ); } );
        "we expect the context instantiation not to be repeated for the second assertion".Assert( () => { Assert.True( true ); } );
    }

    [Specification]
    public void MultipleObservationsShouldNotCauseActionToBeRepeated()
    {
        var sut = new ContextFixtureSpy();

        "Given an externally managed context".Context( () => { } );
        "when we execute an action on it that may be invoked only once".Do( () => { sut.FailWhenCallingTwice(); } );

        "we expect our first assertion to pass".Observation( () => { Assert.True( true ); } );
        "we expect the action not to be repeated for the second assertion".Observation( () => { Assert.True( true ); } );
    }

    [Specification]
    public void MultipleObservationsShouldNotCauseContextInstantiationToBeRepeated()
    {
        var sut = new ContextFixtureSpy();

        "Given a context that may not be established twice".Context( () => { sut.FailWhenCallingTwice(); } );

        "we expect our first assertion to pass".Observation( () => { Assert.True( true ); } );
        "we expect the context instantiation not to be repeated for the second assertion".Observation( () => { Assert.True( true ); } );
    }

    public static void SpecificationThatShouldDisposeItsAssertionFixture()
    {
        "Given a disposable Fixture".ContextFixture( () => new DisposeSpy() );
        "when we encounter an exception during test execution ".Do( () => { throw new Exception( "" ); } );
        "with Assertions, we expect the context is nonetheless disposed".Assert( () => { } );
    }

    [Fact]
    public void ErrorInDoForAssertionDisposesContext()
    {
        DisposeSpy.Reset();

        IMethodInfo method = Reflector.Wrap( StaticReflection.MethodOf( () => SpecificationThatShouldDisposeItsAssertionFixture() ) );

        ExecuteSpecification( method );

        Assert.True( DisposeSpy.WasDisposed );
    }

    public static void SpecificationThatShouldDisposeItsObservationFixture()
    {
        "Given a disposable Fixture".ContextFixture( () => new DisposeSpy() );
        "when we encounter an exception during test execution ".Do( () => { throw new Exception( "" ); } );
        "with Observation, we expect the context is nonetheless disposed".Observation( () => { } );
    }

    [Fact]
    public void ErrorInDoForObservationDisposesContext()
    {
        DisposeSpy.Reset();

        IMethodInfo method = Reflector.Wrap( StaticReflection.MethodOf( () => SpecificationThatShouldDisposeItsObservationFixture() ) );
        ExecuteSpecification( method );

        Assert.True( DisposeSpy.WasDisposed );
    }

    private static void ExecuteSpecification( IMethodInfo spec )
    {
        foreach (var item in SpecificationAttribute.FtoEnumerateTestCommands( spec ))
            item.Execute( null );
    }

    private class DisposeSpy : IDisposable
    {
        private static bool Disposed = false;

        public static bool WasDisposed
        {
            get { return Disposed; }
        }

        public static void Reset()
        {
            Disposed = false;
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}