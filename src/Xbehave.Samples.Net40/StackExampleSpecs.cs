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
        public void Push(int element)
        {
            var stack = default(Stack<int>);

            ("Given the element " + element.ToString())
                .Given(() => { });

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
