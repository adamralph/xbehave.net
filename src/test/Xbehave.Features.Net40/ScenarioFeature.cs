// <copyright file="ScenarioFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
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
    public class ScenarioFeature
    {
        private static object[] arguments;
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

            "Then each result should be a failure"
                .Then(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And only the first step should have been executed"
                .And(() => executedStepCount.Should().Be(1));

            "And each subsequent result message should indicate that the step failed because of failure to execute the first step"
                .And(() => results.Cast<FailedResult>().Skip(1).Should()
                    .OnlyContain(result => result.Message.Contains("Failed to execute preceding step \"[01.01.01] Given something\"")));
        }

        [Scenario]
        public static void FailingStepAfterContinueOnFailureStepType()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a failing step after the first Then"
                .Given(() => feature = typeof(FeatureWithAFailingStepAfterContinueOnFailureStepType));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then the first 3 should be passes"
                .Then(() => results.Take(3).Should().ContainItemsAssignableTo<PassedResult>());

            "And the 4th result should be a failure"
                .And(() => results[3].Should().BeAssignableTo<FailedResult>());

            "And the rest should be passes"
                .Then(() => results.Skip(4).Should().ContainItemsAssignableTo<PassedResult>().And.NotBeEmpty());

            "And it should execute all the steps"
                .And(() => executedStepCount.Should().Be(results.Length));
        }

        [Scenario]
        public static void FailingStepBeforeContinueOnFailureStepType()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a failing step after the first Then (but before the first But)"
                .Given(() => feature = typeof(FeatureWithAFailingStepBeforeContinueOnFailureStepType));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then the first 3 should be passes"
                .Then(() => results.Take(3).Should().ContainItemsAssignableTo<PassedResult>());

            "And the 4th result should be a failure"
                .And(() => results[3].Should().BeAssignableTo<FailedResult>());

            "And the rest should be failures"
                .Then(() => results.Skip(4).Should().ContainItemsAssignableTo<FailedResult>().And.NotBeEmpty());

            "And each subsequent result message should indicate that the step failed because of failure to execute the 4th step"
                .And(() => results.Skip(4).Cast<FailedResult>()
                                  .Should()
                                  .OnlyContain(result => result.Message.Contains("Failed to execute preceding step \"[01.01.04] And something goes wrong\"")));

            "And it should execute 4 steps"
                .And(() => executedStepCount.Should().Be(4));
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

        [Scenario]
        public static void ScenarioWithParameters()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with a single step and parameters"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndParameters));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then each result should be a success"
                .Then(() => results.Should().ContainItemsAssignableTo<PassedResult>());

            "Then the scenario should be executed with default values passed as arguments"
                .Then(() => arguments.Should().OnlyContain(obj => (int)obj == default(int)))
                .Teardown(() => arguments = null);
        }

        private static class FeatureWithAScenarioWithASingleStepAndParameters
        {
            [Scenario]
            public static void Scenario(int w, int x, int y, int z)
            {
                "Given {0}, {1}, {2} and {3}"
                    .Given(() => arguments = new object[] { w, x, y, z });
            }
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
            [Scenario]
            [Example("a")]
            public static void Scenario2(int i)
            {
                "Given"
                    .Given(() => ++executedStepCount);

                "When"
                    .When(() => ++executedStepCount);
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
