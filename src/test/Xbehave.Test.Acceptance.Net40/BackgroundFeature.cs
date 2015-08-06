// <copyright file="BackgroundFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    // In order to write less code
    // As a developer
    // I want to add background steps to all the scenarios in a feature
    public class BackgroundFeature : Feature
    {
        [Scenario]
        [Example(typeof(BackgroundWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        [Example(typeof(BackgroundInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        public void BackgroundSteps(Type feature, ITestResultMessage[] results)
        {
            "Given a {0}"
                .f(() => { });

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then the background steps are run before each scenario"
                .f(() => results.Should().ContainItemsAssignableTo<ITestPassed>());

            "And there are eight results"
                .f(() => results.Length.Should().Be(8));

            "And the background steps have '(Background)' in their names"
                .f(() =>
                {
                    foreach (var result in results.Take(2).Concat(results.Skip(4).Take(2)))
                    {
                        result.Test.DisplayName.Should().Contain("(Background)");
                    }
                });
        }

        private static class BackgroundWithTwoStepsAndTwoScenariosEachWithTwoSteps
        {
            private static int x;

            [Background]
            public static void Background()
            {
                "Given x is incremented"
                    .f(() => ++x);

                "And x is incremented again"
                    .f(() => ++x);
            }

            [Scenario]
            public static void Scenario1()
            {
                "Given x is 2"
                    .f(() => x.Should().Be(2));

                "Then I set x to 0"
                    .f(() => x = 0);
            }

            [Scenario]
            public static void Scenario2()
            {
                "Given x is 2"
                    .f(() => x.Should().Be(2));

                "Then I set x to 0"
                    .f(() => x = 0);
            }
        }

        private class Base
        {
            protected int X { get; set; }

            [Background]
            public void Background()
            {
                "Given x is incremented"
                    .f(() => ++this.X);

                "And x is incremented again"
                    .f(() => ++this.X);
            }
        }

        private class BackgroundInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps : Base
        {
            [Scenario]
            public void Scenario1()
            {
                "Given x is 2"
                    .f(() => this.X.Should().Be(2));

                "Then I set x to 0"
                    .f(() => this.X = 0);
            }

            [Scenario]
            public void Scenario2()
            {
                "Given x is 2"
                    .f(() => this.X.Should().Be(2));

                "Then I set x to 0"
                    .f(() => this.X = 0);
            }
        }
    }
}
