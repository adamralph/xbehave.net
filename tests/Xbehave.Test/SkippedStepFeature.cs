namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Infrastructure;
    using Xunit.Abstractions;

    // In order to commit nearly completed features
    // As a developer
    // I want to temporarily skip specific steps
    public class SkippedStepFeature : Feature
    {
        [Scenario]
        public void UnfinishedFeature(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with skipped steps because \"the feature is unfinished\""
                .x(() => feature = typeof(AScenarioWithSkippedStepsBecauseTheFeatureIsUnfinished));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the results should not be empty"
                .x(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .x(() => results.Should().NotContain(result => result is ITestFailed));

            "And some steps should have been skipped"
                .x(() => results.Any(result => result is ITestSkipped).Should().BeTrue());

            "And each skipped step should be skipped because \"the feature is unfinished\""
                .x(() => results.OfType<ITestSkipped>().Should().OnlyContain(result =>
                    result.Reason == "the feature is unfinished"));
        }

        private static class AScenarioWithSkippedStepsBecauseTheFeatureIsUnfinished
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .x(() => { });

                "When I doing something"
                    .x(() => { });

                "Then there is an outcome"
                    .x(() => throw new NotImplementedException()).Skip("the feature is unfinished");

                "And there is another outcome"
                    .x(() => throw new NotImplementedException()).Skip("the feature is unfinished");
            }
        }
    }
}
