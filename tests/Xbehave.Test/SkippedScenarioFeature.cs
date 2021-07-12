using System;
using Xbehave.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test
{
    // In order to commit largely incomplete features
    // As a developer
    // I want to temporarily skip an entire scenario
    public class SkippedScenarioFeature : Feature
    {
        [Scenario]
        public void SkippedScenario(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a skipped scenario"
                .x(() => feature = typeof(FeatureWithASkippedScenario));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .x(() => Assert.Single(results));

            "And the result should be a skip result"
                .x(() => Assert.IsAssignableFrom<ITestSkipped>(results[0]));
        }

        private static class FeatureWithASkippedScenario
        {
            [Scenario(Skip = "Test")]
            public static void Scenario1() => throw new InvalidOperationException();
        }
    }
}
