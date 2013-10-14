// <copyright file="FluentSyntax.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Samples.Fixtures;

    public class FluentSyntax
    {
        [Scenario]
        public void Addition(int x, int y, Calculator calculator, int answer)
        {
            _
            .Given("Given the number 1", () => x = 1)
            .And("And the number 2", () => y = 2)
            .And("And a calculator", () => calculator = new Calculator())
            .When("When I add the numbers together", () => answer = calculator.Add(x, y))
            .Then("Then the answer is 3", () => answer.Should().Be(3));
        }
    }
}
