// <copyright file="SkippedStackSpecs.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Xbehave;

    public class SkippedStackSpecs
    {
        [Scenario]
        public void Push(int element, Stack<int> stack)
        {
            "Given an element"
                .Given(() => element = 11);

            "And a stack"
                .And(() => stack = new Stack<int>());

            "when pushing the element"
                .When(() => stack.Push(element));

            "then the stack should not be empty"
                .Then(() => stack.Should().NotBeEmpty()).Skip("Just for fun.");

            "then the stack peek should be the element"
                .Then(() => stack.Peek().Should().Be(element)).Skip("Just for fun.");
        }
    }
}
