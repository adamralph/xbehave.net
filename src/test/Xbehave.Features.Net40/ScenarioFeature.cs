﻿// <copyright file="ScenarioFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Features.Infrastructure;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;

    // In order to prevent bugs due to incorrect code
    // As a developer
    // I want to run automated acceptance tests describing each feature of my product using scenarios
    public static class ScenarioFeature
    {
        private static object[] arguments;
        private static int executedStepCount;

        [Fact(Skip = "Temporary development aid for use whilst working on getting scenarios recognised by the runner.")]
        public static void ScenarioBodyThrowsAnExceptionShouldResultInAFailure()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(Result[]);

            feature = typeof(FeatureWithAScenarioBodyWhichThrowsAnException);

            exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray());

            exception.Should().BeNull();

            results.Should().NotBeEmpty();

            results.Should().ContainItemsAssignableTo<Fail>();
        }

        [Scenario]
        public static void ScenarioBodyThrowsAnException()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(Result[]);

            "Given a feature with a scenario body which throws an exception"
                .Given(() => feature = typeof(FeatureWithAScenarioBodyWhichThrowsAnException));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should not be empty"
                .And(() => results.Should().NotBeEmpty());

            "And each result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<Fail>());
        }

        [Scenario]
        public static void FeatureCannotBeConstructed()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(Result[]);

            "Given a feature with a non-static scenario but no default constructor"
                .Given(() => feature = typeof(FeatureWithANonStaticScenarioButNoDefaultConstructor));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should not be empty"
                .And(() => results.Should().NotBeEmpty());

            "And each result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<Fail>());
        }

        [Scenario]
        public static void FailingStep()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a failing step followed by passing steps"
                .Given(() => feature = typeof(FeatureWithAFailingStepFollowedByTwoPassingSteps));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => executedStepCount = 0);

            "Then each result should be a failure"
                .Then(() => results.Should().ContainItemsAssignableTo<Fail>());

            "And only the first step should have been executed"
                .And(() => executedStepCount.Should().Be(1));

            "And each subsequent result message should indicate that the step failed because of failure to execute the first step"
                .And(() => results.Cast<Fail>().Skip(1).Should()
                    .OnlyContain(result => result.Message.Contains("Failed to execute preceding step \"[01.01.01] Given something\"")));
        }

        [Scenario]
        public static void FailingStepAfterContinueOnFailureStepType()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a failing step after the first Then"
                .Given(() => feature = typeof(FeatureWithAFailingStepAfterContinueOnFailureStepType));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray())
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
                .When(() => results = TestRunner.Run(feature).ToArray())
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
                                  .OnlyContain(result => result.Message.Contains("Failed to execute preceding step \"[01.01.04] And something goes wrong\"")));

            "And it should execute 4 steps"
                .And(() => executedStepCount.Should().Be(4));
        }

        [Scenario]
        public static void FailingScenario()
        {
            var feature = default(Type);
            var results = default(Result[]);

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
                .And(() => results[0].Should().BeOfType<Fail>());
        }

        [Scenario]
        public static void ScenarioWithParameters()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with a single step and parameters"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndParameters));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then each result should be a success"
                .Then(() => results.Should().ContainItemsAssignableTo<Pass>());

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

            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for testing.")]
            [Scenario]
            public void Scenario()
            {
                "Given something"
                    .Given(() => { });
            }
        }
    }
}
