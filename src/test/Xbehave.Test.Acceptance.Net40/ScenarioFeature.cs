// <copyright file="ScenarioFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using System.Threading;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;
    using Xunit.Sdk;

    // In order to prevent bugs due to incorrect code
    // As a developer
    // I want to run automated acceptance tests describing each feature of my product using scenarios
    public static class ScenarioFeature
    {
        private static int executedStepCount;

        [Scenario]
        public static void ScenarioBodyThrowsAnException()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(MethodResult[]);

            "Given a feature with a scenario body which throws an exception"
                .Given(() => feature = typeof(FeatureWithAScenarioBodyWhichThrowsAnException));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should not be empty"
                .And(() => results.Should().NotBeEmpty());

            "And each result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());
        }

        [Scenario]
        public static void FeatureCannotBeConstructed()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(MethodResult[]);

            "Given a feature with a non-static scenario but no a default constructor"
                .Given(() => feature = typeof(FeatureWithANonStaticScenarioButNoDefaultConstructor));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should not be empty"
                .And(() => results.Should().NotBeEmpty());

            "And each result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());
        }

        [Scenario]
        public static void FailingStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a failing step followed by passing steps"
                .Given(() => feature = typeof(FeatureWithAFailingStepFollowedByTwoPassingSteps));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then each result should a be a failure"
                .Then(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And only the first step should have been executed"
                .And(() => executedStepCount.Should().Be(1));

            "And each subsequent result message should indicate that the step failed because of failure to execute the first step"
                .And(() => results.Cast<FailedResult>().Skip(1).Should()
                    .OnlyContain(result => result.Message.Contains("Failed to execute preceding step \"[01.01] Given something\"")));
        }

        [Scenario]
        public static void SkippedScenario()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a skipped scenario"
                .Given(() => feature = typeof(FeatureWithASkippedScenario));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then no steps should have been executed"
                .Then(() => executedStepCount.Should().Be(0));

            "And there should be one result"
                .And(() => results.Count().Should().Be(1));

            "And the result should be a skip result"
                .And(() => results[0].Should().BeOfType<SkipResult>());
        }

        [Scenario]
        public static void FailingScenario()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a failing scenario"
                .Given(() => feature = typeof(FeatureWithAFailingScenario));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then no steps should have been executed"
                .Then(() => executedStepCount.Should().Be(0));

            "And there should be one result"
                .And(() => results.Count().Should().Be(1));

            "And the result should be a failed result"
                .And(() => results[0].Should().BeOfType<FailedResult>());
        }

        private static class FeatureWithASkippedScenario
        {
            [Scenario(Skip = "Test")]
            public static void Scenario1()
            {
                "Given"
                    .Given(() => ++executedStepCount);

                "When"
                    .When(() => ++executedStepCount);
            }
        }

        private static class FeatureWithAFailingScenario
        {
            [Scenario(Timeout = 100)]
            public static void Scenario2()
            {
                "Given"
                    .Given(() => ++executedStepCount);

                "When"
                    .When(() => ++executedStepCount);

                Thread.Sleep(200);
            }
        }

        private static class FeatureWithAScenarioBodyWhichThrowsAnException
        {
            [Scenario]
            public static void Scenario()
            {
                throw new InvalidOperationException();
            }
        }

        private static class FeatureWithAFailingStepFollowedByTwoPassingSteps
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() =>
                    {
                        ++executedStepCount;
                        throw new NotImplementedException();
                    });

                "When something happens"
                    .When(() => ++executedStepCount);

                "Then there is an outcome"
                    .Then(() => ++executedStepCount);
            }
        }

        private class FeatureWithANonStaticScenarioButNoDefaultConstructor
        {
            public FeatureWithANonStaticScenarioButNoDefaultConstructor(int ignored)
            {
            }

            [Scenario]
            public void Scenario()
            {
                "Given something"
                    .Given(() => { });
            }
        }
    }
}
