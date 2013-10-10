// <copyright file="IsolatedSteps.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Samples.Fixtures;

    public class IsolatedSteps
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

            "Then the answer is one less than 4"
                .Then(() => (++answer).Should().Be(4))
                .InIsolation();

            "And the answer is 3"
                .And(() => answer.Should().Be(3));
        }
    }
}
