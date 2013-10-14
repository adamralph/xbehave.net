// <copyright file="TheLot.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Samples.Fixtures;

    public static class TheLot
    {
        private static Calculator calculator;

        [Background]
        public static void Background()
        {
            "Given a stack"
                .Given(() => calculator = new Calculator())
                .Teardown(() => calculator.CoolDown());
        }

        [Scenario]
        [Example(1, 2, 3)]
        [Example(2, 3, 5)]
        public static void Addition(int x, int y, int expectedAnswer, Calculator calculator, int answer)
        {
            "Given the number {0}"
                .Given(() => { });

            "And the number {1}"
                .And(() => { });

            "And a calculator"
                .And(() => calculator = new Calculator());

            "And some disposable object"
                .And(() => new Disposable().Using());

            "When I add the numbers together"
                .When(() => answer = calculator.Add(x, y))
                .WithTimeout(1000);

            "Then the answer is not more than {2}"
                .Then(() => (++answer).Should().Be(expectedAnswer + 1))
                .InIsolation();

            "And the answer is {2}"
                .And(() => answer.Should().Be(expectedAnswer));

            "And the answer is one more than {2}"
                .And(() => answer.Should().Be(expectedAnswer + 1))
                .Skip("because the assertion is nonsense");

            "But the answer is not one less than {2}"
                .But(() => answer.Should().NotBe(expectedAnswer - 1));
        }
    }
}
