// <copyright file="SkippedScenarioFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    // In order to commit largely incomplete features
    // As a developer
    // I want to temporarily skip an entire scenario
    public class SkippedScenarioFeature : Feature
    {
        [Scenario]
        public void SkippedScenario()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a feature with a skipped scenario"
                .f(() => feature = typeof(FeatureWithASkippedScenario));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .f(() => results.Count().Should().Be(1));

            "And the result should be a skip result"
                .f(() => results[0].Should().BeAssignableTo<ITestSkipped>(
                    results.ToDisplayString("the result should be a skip")));
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
