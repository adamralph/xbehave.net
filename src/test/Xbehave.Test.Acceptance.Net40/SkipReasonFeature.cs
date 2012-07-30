// <copyright file="SkipReasonFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to avoid analysing skipped steps
    // As a developer
    // I want to provide a reason when I skip a step
    public static class SkipReasonFeature
    {
        [Scenario]
        public static void SkippingAStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with scenarios with skipped steps because \"the feature is unfinished\""
                .Given(() => feature = typeof(FeatureWithScenariosWithSkippedStepsBecauseTheFeatureIsUnfinished));

            "When the test runner executes the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the steps should be skipped because \"the feature is unfinished\""
                .Then(() =>
                    {
                        results.Should().ContainItemsAssignableTo<SkipResult>();
                        results.Cast<SkipResult>().Should().OnlyContain(result => result.Reason == "the feature is unfinished");
                    });
        }

        private static class FeatureWithScenariosWithSkippedStepsBecauseTheFeatureIsUnfinished
        {
            [Scenario]
            public static void Scenario()
            {
                "Then there is an outcome"
                    .Then(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");

                "And there is another outcome"
                    .And(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");
            }
        }
    }
}
