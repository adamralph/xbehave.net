// <copyright file="TheLot.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave;

    public class TheLot
    {
        private static Stack<int> stack;

        [Background]
        public static void Background()
        {
            "Given a stack"
                .Given(() => stack = new Stack<int>());
        }

        [Scenario]
        [Example(123)]
        [Example(234)]
        public static void Push(int element)
        {
            "Given {0}"
                .Given(() => new Disposable().Using());

            "When pushing {0} onto the stack"
                .When(() => stack.Push(element))
                .Teardown(() => stack.Clear())
                .And()
                .WithTimeout(1000);

            "Then the stack should not be empty"
                .Then(() => stack.Should().NotBeEmpty());

            "And the stack pop should be {0}"
                .And(() => stack.Pop().Should().Be(element))
                .InIsolation()
                .And()
                .WithTimeout(1000);

            "And the stack peek should be {0}"
                .And(() => stack.Peek().Should().Be(element))
                .WithTimeout(1000);

            "And the stack count should be 3"
                .And(() => stack.Count().Should().Be(2))
                .Skip("because the assertion is nonsense");

            "But the stack count should not be 2"
                .But(() => stack.Count().Should().NotBe(2));
        }
    }
}
