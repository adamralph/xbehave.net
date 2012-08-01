// <copyright file="SkippedStepFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to commit unfinished features
    // As a developer
    // I want to temporarily skip specific steps
    public static class SkippedStepFeature
    {
        [Scenario]
        public static void UnfinishedFeature()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with skipped steps because \"the feature is unfinished\" which would throw exceptions if executed"
                .Given(() => feature = typeof(FeatureWithAScenarioWithSkippedStepsBecauseTheFeatureIsUnfinishedWhichWouldThrowExceptionsIfExecuted));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .And(() => results.Should().NotContain(result => result is FailedResult));

            "And some steps should have been skipped"
                .And(() => results.Any(result => result is SkipResult).Should().BeTrue());

            "And each skipped step should be skipped because \"the feature is unfinished\""
                .And(() => results.OfType<SkipResult>().Should().OnlyContain(result => result.Reason == "the feature is unfinished"));
        }

        private static class FeatureWithAScenarioWithSkippedStepsBecauseTheFeatureIsUnfinishedWhichWouldThrowExceptionsIfExecuted
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { });

                "When I doing something"
                    .When(() => { });

                "Then there is an outcome"
                    .Then(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");

                "And there is another outcome"
                    .And(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");
            }
        }
    }
}
