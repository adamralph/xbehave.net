// <copyright file="SubSpecDemo.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Legacy
{
    using System.Collections.Generic;
    using System.Threading;
    using Xunit;

    public static class SubSpecDemo
    {
        [Specification]
        public static void SpecificationSyntax()
        {
            var sut = default(Stack<int>);
            "Given a new stack"
                .Context(
                () => sut = new Stack<int>());

            var element = 11;
            "with an element pushed onto it"
                .Do(
                () => sut.Push(element));

            "expect the stack is not empty"
                .Observation(
                () => Assert.NotEmpty(sut));

            "expect peek gives us the top element"
                .Observation(
                () => Assert.Equal(element, sut.Peek()));

            "expect pop gives us the top element"
                .Assert(
                () => Assert.Equal(element, sut.Pop()));

            "expect pop in another Assertions still gives us the top element"
                .Assert(
                () => Assert.Equal(element, sut.Pop()));
        }

        [Specification(Skip = "Not relevant for CI.")]
        public static void FluentTimeouts()
        {
            // You can have individual timeouts per specification primitive
            // Change the sleep time or timeouts to see them fail
            "Given a context that should not take longer than 1000ms  to establish".Context(() => Thread.Sleep(10)).WithTimeout(1000);
            "When we do something that should not take longer than 1000ms ".Do(() => Thread.Sleep(10)).WithTimeout(1000);
            "Assert something within 1000ms ".Assert(() => Thread.Sleep(10)).WithTimeout(1000);
            "Observe something within 1000ms ".Observation(() => Thread.Sleep(10)).WithTimeout(1000);
            "Observe something within 1001ms".Observation(() => Thread.Sleep(11)).WithTimeout(10001);
        }

        ////[Specification]
        ////public void ComppositeFixtures()
        ////{
        ////    // You can have a Composite Fixture made up of several items easily
        ////    var bar = default( IDisposable );
        ////    var foo = default( IDisposable );

        ////    "Given a foo and a bar"
        ////        .ContextFixture(
        ////            () => foo = new DisposableAction( () => Console.WriteLine( "foo disposed" ) ),
        ////            () => bar = new DisposableAction( () => Console.WriteLine( "bar disposed" ) ) );

        ////    "observe foo exists"
        ////        .Observation( () => Assert.NotNull( foo ) );
        ////    "observe bar exists"
        ////        .Observation( () => Assert.NotNull( foo ) );

        ////    // Output: 
        ////    // bar disposed
        ////    // foo diposed
        ////}

        ////[Specification]
        ////public void ExpressionSyntax()
        ////{
        ////    const int element = 11;

        ////    var spec = from sut in SpecificationExpression.Begin( () => new Stack<int>() )
        ////               where sut.Push( element )
        ////               select new Observations() 
        ////    {
        ////        () => Assert.NotEmpty(sut), 
        ////        () => Assert.Equal(element, sut.Peek()) 
        ////    };
        ////}
    }
}