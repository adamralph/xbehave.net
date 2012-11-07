// <copyright file="FluentStackSpecs.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave;

    public class FluentStackSpecs
    {
        [Scenario]
        public void Push(int element, Stack<int> stack)
        {
            _
            .Given("an element", () => element = 11)
            .And("a stack", () => stack = new Stack<int>())
            .When("pushing the element", () => stack.Push(element))
            .Then("the stack should not be empty", () => stack.Should().NotBeEmpty())
            .And("the stack peek should be the element", () => stack.Peek().Should().Be(element))
            .And("the stack peek should be the element", () => stack.Peek().Should().Be(element)).InIsolation()
            .And("the stack peek should be the element", () => stack.Peek().Should().Be(element)).Skip("because I can");
        }
    }
}
