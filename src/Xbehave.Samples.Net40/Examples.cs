// <copyright file="Examples.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Samples.Fixtures;

    public class Examples
    {
        [Scenario]
        [Example(1, 2, 3)]
        [Example(2, 3, 5)]
        public void Addition(int x, int y, int expectedAnswer, Calculator calculator, int answer)
        {
            "Given the number {0}"
                .Given(() => { });

            "And the number {1}"
                .And(() => { });

            "And a calculator"
                .And(() => calculator = new Calculator());

            "When I add the numbers together"
                .When(() => answer = calculator.Add(x, y));

            "Then the answer is {2}"
                .Then(() => answer.Should().Be(expectedAnswer));
        }
    }
}
