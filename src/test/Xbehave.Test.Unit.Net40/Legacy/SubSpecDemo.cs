// <copyright file="SubSpecDemo.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Legacy
{
    using System.Collections.Generic;
    using System.Threading;
    using Xunit;

    public static class SubSpecDemo
    {
        [Scenario]
        public static void SpecificationSyntax()
        {
            var sut = default(Stack<int>);
            "Given a new stack"
                .Given(() => sut = new Stack<int>());

            var element = 11;
            "with an element pushed onto it"
                .When(() => sut.Push(element));

            "expect the stack is not empty"
                .Then(() => Assert.NotEmpty(sut));

            "expect peek gives us the top element"
                .Then(() => Assert.Equal(element, sut.Peek()));

            "expect pop gives us the top element"
                .Then(() => Assert.Equal(element, sut.Pop()))
                .InIsolation();

            "expect pop in another Assertions still gives us the top element"
                .Then(() => Assert.Equal(element, sut.Pop()))
                .InIsolation();
        }

        [Scenario(Skip = "Not relevant for CI.")]
        public static void FluentTimeouts()
        {
            // You can have individual timeouts per specification primitive
            // Change the sleep time or timeouts to see them fail
            "Given a context that should not take longer than 1000ms  to establish"
                .Given(() => Thread.Sleep(10))
                .WithTimeout(1000);
            
            "When we do something that should not take longer than 1000ms "
                .When(() => Thread.Sleep(10))
                .WithTimeout(1000);
            
            "Assert something within 1000ms "
                .Then(() => Thread.Sleep(10))
                .WithTimeout(1000)
                .InIsolation();
            
            "Observe something within 1000ms "
                .Then(() => Thread.Sleep(10))
                .WithTimeout(1000);
            
            "Observe something within 1001ms"
                .Then(() => Thread.Sleep(11))
                .WithTimeout(10001);
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