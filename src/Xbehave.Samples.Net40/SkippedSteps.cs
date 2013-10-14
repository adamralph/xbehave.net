// <copyright file="SkippedSteps.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Samples.Fixtures;

    public class SkippedSteps
    {
        [Scenario]
        public void Addition(int x, int y, Calculator calculator, int answer)
        {
            "Given the number 1"
                .Given(() => x = 1);

            "And the number 2"
                .And(() => y = 2);

            "And a calculator"
                .And(() => calculator = new Calculator());

            "When I add the numbers together"
                .When(() => answer = calculator.Add(x, y));

            "Then the answer is 3"
                .Then(() => answer.Should().Be(3))
                .Skip("I've almost got this addition thing working, I just can't quite get the right numbers out.");
        }
    }
}
