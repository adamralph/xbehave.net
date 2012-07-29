// <copyright file="SkipReasonFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System.Linq;
    using FluentAssertions;
    using Xunit.Sdk;

    // In order to avoid analysing skipped steps
    // As a developer
    // I want to provide a reason when I skip a step
    public static class SkipReasonFeature
    {
        [Scenario]
        public static void SkippingAStep()
        {
            var scenario = default(IMethodInfo);
            var results = default(MethodResult[]);

            "Given a scenario with skipped steps because \"the feature is unfinished\""
                .Given(() => scenario = TestRunner.CreateScenario(ScenarioWithASkippedStepBecauseTheFeatureIsUnfinished));

            "When the test runner executes the scenario"
                .When(() => results = TestRunner.Execute(scenario).ToArray());

            "Then the steps should be skipped because \"the feature is unfinished\""
                .Then(() =>
                    {
                        results.Should().ContainItemsAssignableTo<SkipResult>();
                        results.Cast<SkipResult>().Should().OnlyContain(result => result.Reason == "the feature is unfinished");
                    });
        }

        public static void ScenarioWithASkippedStepBecauseTheFeatureIsUnfinished()
        {
            "Then there is an outcome"
                .Then(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");

            "And there is another outcome"
                .And(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");
        }
    }
}
