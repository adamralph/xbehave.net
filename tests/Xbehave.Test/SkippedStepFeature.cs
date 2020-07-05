namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using Xbehave.Test.Infrastructure;
    using Xunit;
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
                .x(() => Assert.NotEmpty(results));

            "And there should be no failures"
                .x(() => Assert.Empty(results.OfType<ITestFailed>()));

            "And some steps should have been skipped"
                .x(() => Assert.NotEmpty(results.OfType<ITestSkipped>()));

            "And each skipped step should be skipped because \"the feature is unfinished\""
                .x(() => Assert.All(results.OfType<ITestSkipped>(), result => Assert.Equal("the feature is unfinished", result.Reason)));
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
