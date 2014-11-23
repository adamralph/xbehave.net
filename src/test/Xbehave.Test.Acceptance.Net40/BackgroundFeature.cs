// <copyright file="BackgroundFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to write less code
    // As a developer
    // I want to add background steps to all the scenarios in a feature
    public static class BackgroundFeature
    {
        [Scenario]
        [Example(typeof(BackgroundWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        [Example(typeof(BackgroundInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        public static void BackgroundSteps(Type feature, Result[] results)
        {
            "Given a {0}"
                .f(() => { });

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then the background steps are run before each scenario"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>());
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
