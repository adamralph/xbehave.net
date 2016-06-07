// <copyright file="SkippedStepFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    // In order to commit nearly completed features
    // As a developer
    // I want to temporarily skip specific steps
    public class SkippedStepFeature : Feature
    {
        [Scenario]
        public void UnfinishedFeature()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a scenario with skipped steps because \"the feature is unfinished\""
                .f(() => feature = typeof(AScenarioWithSkippedStepsBecauseTheFeatureIsUnfinished));

            "When I run the scenario"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then the results should not be empty"
                .f(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .f(() => results.Should().NotContain(result => result is ITestFailed));

            "And some steps should have been skipped"
                .f(() => results.Any(result => result is ITestSkipped).Should().BeTrue());

            "And each skipped step should be skipped because \"the feature is unfinished\""
                .f(() => results.OfType<ITestSkipped>().Should().OnlyContain(result =>
                    result.Reason == "the feature is unfinished"));
        }

        private static class AScenarioWithSkippedStepsBecauseTheFeatureIsUnfinished
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When I doing something"
                    .f(() => { });

                "Then there is an outcome"
                    .f(() => { throw new NotImplementedException(); }).Skip("the feature is unfinished");

                "And there is another outcome"
                    .f(() => { throw new NotImplementedException(); }).Skip("the feature is unfinished");
            }
        }
    }
}
