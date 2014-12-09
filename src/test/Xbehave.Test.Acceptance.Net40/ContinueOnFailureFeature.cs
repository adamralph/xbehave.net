// <copyright file="ContinueOnFailureFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if !V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    public class ContinueOnFailureFeature : Feature
    {
        private static int executedStepCount;

        [Scenario]
        public void FailingStepAfterContinueOnFailureStepType()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a feature with a failing step after the first Then"
                .Given(() => feature = typeof(FeatureWithAFailingStepAfterContinueOnFailureStepType));

            "When I run the scenarios"
                .When(() => results = this.Run<ITestResultMessage>(feature))
                .Teardown(() => executedStepCount = 0);

            "Then the first 3 should be passes"
                .Then(() => results.Take(3).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the 4th result should be a failure"
                .And(() => results[3].Should().BeAssignableTo<ITestFailed>());

            "And the rest should be passes"
                .Then(() => results.Skip(4).Should().ContainItemsAssignableTo<ITestPassed>().And.NotBeEmpty());

            "And it should execute all the steps"
                .And(() => executedStepCount.Should().Be(results.Length));
        }

        [Scenario]
        public void FailingStepBeforeContinueOnFailureStepType()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a feature with a failing step after the first Then (but before the first But)"
                .Given(() => feature = typeof(FeatureWithAFailingStepBeforeContinueOnFailureStepType));

            "When I run the scenarios"
                .When(() => results = this.Run<ITestResultMessage>(feature))
                .Teardown(() => executedStepCount = 0);

            "Then the first 3 should be passes"
                .Then(() => results.Take(3).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the 4th result should be a failure"
                .And(() => results[3].Should().BeAssignableTo<ITestFailed>());

            "And the rest should be failures"
                .Then(() => results.Skip(4).Should().ContainItemsAssignableTo<ITestFailed>().And.NotBeEmpty());

            "And each subsequent result message should indicate that the step failed because of failure to execute the 4th step"
                .And(() => results.Skip(4).Cast<ITestFailed>()
                    .Should()
                    .OnlyContain(result => result.Messages.Single().Contains(
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
