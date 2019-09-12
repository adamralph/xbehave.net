namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Infrastructure;
    using Xunit.Abstractions;

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
                .x(() => results.Count().Should().Be(1));

            "And the result should be a skip result"
                .x(() => results[0].Should().BeAssignableTo<ITestSkipped>(
                    results.ToDisplayString("the result should be a skip")));
        }

        private static class FeatureWithASkippedScenario
        {
#pragma warning disable xUnit1004 // Test methods should not be skipped
            [Scenario(Skip = "Test")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
            public static void Scenario1() => throw new InvalidOperationException();
        }
    }
}
