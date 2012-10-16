// <copyright file="StackExampleSpecs.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave;

    public class StackExampleSpecs
    {
        [Scenario]
        [Example(123)]
        [Example(234)]
        public void Push(int element, Stack<int> stack)
        {
            "Given {0}"
                .Given(() => { });

            "And a stack"
                .And(() => stack = new Stack<int>());

            "When pushing {0} onto the stack"
                .When(() => stack.Push(element));

            "Then the stack should not be empty"
                .Then(() => stack.Should().NotBeEmpty());

            "And the stack peek should be {0}"
                .And(() => stack.Peek().Should().Be(element));
        }
    }
}
