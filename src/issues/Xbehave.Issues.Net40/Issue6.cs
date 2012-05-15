// <copyright file="Issue6.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/6/replace-skip-and-inisolation-with-fluent
    /// </summary>
    public class Issue6
    {
        [Scenario]
        public void Push()
        {
            var target = default(Stack<int>);
            var element = default(int);

            "Given an element"
                .Given(() => element = 11);

            "and a stack"
                .And(() => target = new Stack<int>());

            "when pushing the element"
                .When(() => target.Push(element));

            "then the target pop should be the element"
                .Then(() => target.Pop().Should().Be(element))
                .InIsolation();

            "then the target should not be empty"
                .Then(() => target.Should().NotBeEmpty());

            "and the target count should be 2"
                .And(() => target.Count.Should().Be(2))
                .Skip("because the target count is 1");
        }

        [Scenario]
        public void PushFluent()
        {
            var target = default(Stack<int>);
            var element = default(int);
            _
            .Given("an element", () => element = 11)
                .And("a stack", () => target = new Stack<int>())
            .When("pushing the element", () => target.Push(element))
            .Then("the target pop should be the element", () => target.Pop().Should().Be(element))
                .InIsolation()
            .Then("the target should not be empty", () => target.Should().NotBeEmpty())
                .And("the target count should be 2", () => target.Count.Should().Be(2))
                    .Skip("because the target count is 1");
        }
    }
}
