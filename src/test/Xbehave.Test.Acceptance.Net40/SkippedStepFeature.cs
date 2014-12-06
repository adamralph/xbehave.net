// <copyright file="SkippedStepFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to commit nearly completed features
    // As a developer
    // I want to temporarily skip specific steps
    public static class SkippedStepFeature
    {
        [Scenario]
        public static void UnfinishedFeature()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with skipped steps because \"the feature is unfinished\" which would throw exceptions if executed"
                .f(() => feature = typeof(FeatureWithAScenarioWithSkippedStepsBecauseTheFeatureIsUnfinishedWhichWouldThrowExceptionsIfExecuted));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then the results should not be empty"
                .f(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .f(() => results.Should().NotContain(result => result is Fail));

            "And some steps should have been skipped"
                .f(() => results.Any(result => result is Skip).Should().BeTrue());

            "And each skipped step should be skipped because \"the feature is unfinished\""
                .f(() => results.OfType<Skip>().Should().OnlyContain(result => result.Reason == "the feature is unfinished"));
        }

        private static class FeatureWithAScenarioWithSkippedStepsBecauseTheFeatureIsUnfinishedWhichWouldThrowExceptionsIfExecuted
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When I doing something"
                    .f(() => { });

                "Then there is an outcome"
                    .f(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");

                "And there is another outcome"
                    .f(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");
            }
        }
    }
}
