// <copyright file="StackSpecs.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave;

    public class StackSpecs
    {
        [Scenario]
        public void Push(int element, Stack<int> stack)
        {
            "Given an element"
                .Given(() => element = 11);

            "And a stack"
                .And(() => stack = new Stack<int>());

            "When pushing the element onto the stack"
                .When(() => stack.Push(element));

            "Then the stack should not be empty"
                .Then(() => stack.Should().NotBeEmpty());

            "And the stack peek should be the element"
                .And(() => stack.Peek().Should().Be(element));
        }
    }
}
