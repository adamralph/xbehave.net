// <copyright file="InIsolation.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave;

    public class InIsolation
    {
        [Scenario]
        public void Push()
        {
            var stack = default(Stack<int>);
            var element = default(int);

            "Given an element"
                .Given(() => element = 11);

            "And a stack"
                .And(() => stack = new Stack<int>());

            "When pushing the element onto the stack"
                .When(() => stack.Push(element));

            "Then the stack pop should be the element"
                .Then(() => stack.Pop().Should().Be(element))
                .InIsolation();

            "And the stack should not be empty"
                .And(() => stack.Should().NotBeEmpty());
        }
    }
}
