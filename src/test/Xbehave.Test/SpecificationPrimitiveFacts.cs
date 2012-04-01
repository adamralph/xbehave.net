using System;
using Xunit;
using Xbehave;
using System.Threading;

//public class SpecificationPrimitiveFacts
//{
//    [Fact]
//    public void CreatePrimitive_WithNullAction_Throws()
//    {
//        Assert.Throws<ArgumentNullException>( () => new SpecificationPrimitive<Action>( "", null ) );
//    }

//    [Fact]
//    public void CreatePrimitive_WithNullMessage_Throws()
//    {
//        Assert.Throws<ArgumentNullException>( () => new SpecificationPrimitive<Action>( null, () => { } ) );
//    }

//    [Fact]
//    public void Execute_Action_WithTimeout_FailsCorrectly()
//    {
//        SpecificationPrimitive<Action> sut = new SpecificationPrimitive<Action>( "", () => Thread.Sleep( 1000 ) );

//        sut.WithTimeout( 10 );

//        Assert.Throws<Xunit.Sdk.TimeoutException>( () => sut.Execute() );
//    }

//    [Fact]
//    public void Execute_Action_Timeout_HappyPath()
//    {
//        SpecificationPrimitive<Action> sut = new SpecificationPrimitive<Action>( "", () => { } );

//        sut.WithTimeout( 10 );

//        Assert.DoesNotThrow( () => sut.Execute() );
//    }

//    [Fact]
//    public void Execute_Action_NoTimeout_HappyPath()
//    {
//        SpecificationPrimitive<Action> sut = new SpecificationPrimitive<Action>( "", () => { } );

//        Assert.DoesNotThrow( () => sut.Execute() );
//    }

//    [Fact]
//    public void Execute_ContextDelegate_WithTimeout_FailsCorrectly()
//    {
//        SpecificationPrimitive<ContextDelegate> sut = new SpecificationPrimitive<ContextDelegate>( "",
//            () =>
//            {
//                Thread.Sleep( 20 );
//                return DisposableAction.None;
//            } );

//        sut.WithTimeout( 10 );

//        Assert.Throws<Xunit.Sdk.TimeoutException>( () => sut.Execute() );
//    }

//    [Fact]
//    public void Execute_ContextDelegate_WithTimeout_HappyPath()
//    {
//        SpecificationPrimitive<ContextDelegate> sut = new SpecificationPrimitive<ContextDelegate>( "", () => DisposableAction.None );

//        sut.WithTimeout( 10 );

//        var result = default( IDisposable );
//        Assert.DoesNotThrow( () => result = sut.Execute() );

//        Assert.Equal( DisposableAction.None, result );
//    }

//    [Fact]
//    public void Execute_ContextDelegate_NoTimeout_HappyPath()
//    {
//        SpecificationPrimitive<ContextDelegate> sut = new SpecificationPrimitive<ContextDelegate>( "", () => DisposableAction.None );

//        var result = default( IDisposable );
//        Assert.DoesNotThrow( () => result = sut.Execute() );

//        Assert.Equal( DisposableAction.None, result );
//    }
//}