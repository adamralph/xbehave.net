// <copyright file="BackgroundFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if !V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to write less code
    // As a developer
    // I want to add background steps to all the scenarios in a feature
    public static class BackgroundFeature
    {
        [Scenario]
        public static void FeatureWithBackgroundSteps(Type feature, Result[] results)
        {
            "Given a background and 2 scenarios"
                .f(() => feature = typeof(Feature));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then the background steps are run before each scenario"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>());
        }

        private static class Feature
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
    }
}
#endif