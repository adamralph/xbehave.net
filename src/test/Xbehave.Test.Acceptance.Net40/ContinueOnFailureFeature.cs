// <copyright file="ContinueOnFailureFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if !V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;

    public static class ContinueOnFailureFeature
    {
        private static int executedStepCount;

        [Scenario]
        public static void FailingStepAfterContinueOnFailureStepType()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a failing step after the first Then"
                .Given(() => feature = typeof(FeatureWithAFailingStepAfterContinueOnFailureStepType));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then the first 3 should be passes"
                .Then(() => results.Take(3).Should().ContainItemsAssignableTo<Pass>());

            "And the 4th result should be a failure"
                .And(() => results[3].Should().BeAssignableTo<Fail>());

            "And the rest should be passes"
                .Then(() => results.Skip(4).Should().ContainItemsAssignableTo<Pass>().And.NotBeEmpty());

            "And it should execute all the steps"
                .And(() => executedStepCount.Should().Be(results.Length));
        }

        [Scenario]
        public static void FailingStepBeforeContinueOnFailureStepType()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a failing step after the first Then (but before the first But)"
                .Given(() => feature = typeof(FeatureWithAFailingStepBeforeContinueOnFailureStepType));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then the first 3 should be passes"
                .Then(() => results.Take(3).Should().ContainItemsAssignableTo<Pass>());

            "And the 4th result should be a failure"
                .And(() => results[3].Should().BeAssignableTo<Fail>());

            "And the rest should be failures"
                .Then(() => results.Skip(4).Should().ContainItemsAssignableTo<Fail>().And.NotBeEmpty());

            "And each subsequent result message should indicate that the step failed because of failure to execute the 4th step"
                .And(() => results.Skip(4).Cast<Fail>()
                    .Should()
                    .OnlyContain(result => result.Message.Contains(
                        "Failed to execute preceding step \"[01.01.04] And something goes wrong\"")));

            "And it should execute 4 steps"
                .And(() => executedStepCount.Should().Be(4));
        }

        private static class FeatureWithAFailingStepAfterContinueOnFailureStepType
        {
            [Scenario]
            [ContinueOnFailureAfter(StepType.Then)]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => ++executedStepCount);

                "When something happens"
                    .When(() => ++executedStepCount);

                "Then there is an outcome"
                    .Then(() => ++executedStepCount);

                "And something goes wrong"
                    .And(() =>
                    {
                        ++executedStepCount;
                        throw new InvalidOperationException("oops");
                    });

                "But this is ok"
                    .But(() => ++executedStepCount);

                "And this is ok"
                    .And(() => ++executedStepCount);
            }
        }

        private static class FeatureWithAFailingStepBeforeContinueOnFailureStepType
        {
            [Scenario]
            [ContinueOnFailureAfter(StepType.But)]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => ++executedStepCount);

                "When something happens"
                    .When(() => ++executedStepCount);

                "Then there is an outcome"
                    .Then(() => ++executedStepCount);

                "And something goes wrong"
                    .And(() =>
                    {
                        ++executedStepCount;
                        throw new InvalidOperationException("oops");
                    });

                "But this is ok"
                    .But(() => ++executedStepCount);

                "And this is ok"
                    .And(() => ++executedStepCount);
            }
        }
    }
}
#endif
