// <copyright file="SkippedScenarioFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to commit largely incomplete features
    // As a developer
    // I want to temporarily skip an entire scenario
    public static class SkippedScenarioFeature
    {
        [Scenario]
        public static void SkippedScenario()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a skipped scenario"
                .f(() => feature = typeof(FeatureWithASkippedScenario));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then there should be one result"
                .f(() => results.Count().Should().Be(1));

            "And the result should be a skip result"
                .f(() => results[0].Should().BeOfType<Skip>(results.ToDisplayString("the result should be a skip")));
        }

        private static class FeatureWithASkippedScenario
        {
            [Scenario(Skip = "Test")]
            public static void Scenario1()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
