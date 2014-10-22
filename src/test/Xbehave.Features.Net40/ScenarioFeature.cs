// <copyright file="ScenarioFeature.cs" company="xBehave.net contributors">
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
        // NOTE (adamralph): a plain xunit fact to prove that plain scenarios work in 2.x
        [Fact]
        public static void ScenarioWithThreePassingStepsYieldsThreePasses()
        {
            // arrange
            var feature = typeof(FeatureWithAScenarioWithThreePassingSteps);

            // act
            var results = TestRunner.Run(feature).ToArray();

            // assert
            results.Length.Should().Be(3);
            results.Should().ContainItemsAssignableTo<Pass>();
        }

        [Scenario]
        public static void ScenarioWithThreeSteps()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with three steps"
                .Given(() => feature = typeof(FeatureWithAScenarioWithThreeSteps));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then there should be three results"
                .And(() => results.Length.Should().Be(3));

            "And the first result should have a display name ending with 'Step 1'"
                .And(() => results[0].DisplayName.Should().EndWith("Step 1"));

            "And the second result should have a display name ending with 'Step 2'"
                .And(() => results[1].DisplayName.Should().EndWith("Step 2"));

            "And the third result should have a display name ending with 'Step 3'"
                .And(() => results[2].DisplayName.Should().EndWith("Step 3"));
        }

        [Scenario]
        public static void ScenarioWithThreePassingSteps()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with three passing steps"
                .Given(() => feature = typeof(FeatureWithAScenarioWithThreePassingSteps));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then there should be three results"
                .And(() => results.Length.Should().Be(3));

            "And each result should be a pass"
                .And(() => results.Should().ContainItemsAssignableTo<Pass>());
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
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then each result should be a failure"
                .Then(() => results.Should().ContainItemsAssignableTo<Fail>());

            "And each subsequent result message should indicate that the first step failed"
                .And(() =>
                {
                    foreach (var result in results.Cast<Fail>().Skip(1))
                    {
                        result.Message.Should().ContainEquivalentOf("Failed to execute preceding step");
                        result.Message.Should().ContainEquivalentOf("Given something");
                    }
                });
        }

        private static class FeatureWithAScenarioWithThreeSteps
        {
            [Scenario]
            public static void Scenario()
            {
                "Step 1"
                    .f(() => { });

                "Step 2"
                    .f(() => { });

                "Step 3"
                    .f(() => { });
            }
        }

        private static class FeatureWithAScenarioWithThreePassingSteps
        {
            [Scenario]
            public static void Scenario()
            {
                var i = 0;

                "Given 1"
                    .f(() => i = 1);

                "When I add 1"
                    .f(() => i += 1);

                "Then I have 2"
                    .f(() => i.Should().Be(2));
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
                        throw new NotImplementedException();
                    });

                "When something happens"
                    .When(() => { });

                "Then there is an outcome"
                    .Then(() => { });
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
