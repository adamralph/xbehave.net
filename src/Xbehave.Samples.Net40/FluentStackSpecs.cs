// <copyright file="FluentStackSpecs.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave;

    public class FluentStackSpecs
    {
        [Scenario]
        public void Push()
        {
            var target = default(Stack<int>);
            var element = default(int);
            _
            .Given("an element", () => element = 11)
            .And("a stack", () => target = new Stack<int>())
            .When("pushing the element", () => target.Push(element))
            .Then("the target should not be empty", () => target.Should().NotBeEmpty())
            .And("the target peek should be the element", () => target.Peek().Should().Be(element))
            .And("the target peek should be the element", () => target.Peek().Should().Be(element)).InIsolation()
            .And("the target peek should be the element", () => target.Peek().Should().Be(element)).Skip("because I can");
        }
    }
}
