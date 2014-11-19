// <copyright file="ScenarioFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FluentAssertions;
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
            var results = feature.RunScenarios();

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

            "When I run the scenarios"
                .When(() => results = feature.RunScenarios());

            "Then there should be three results"
                .And(() => results.Length.Should().Be(3));

            "And the first result should have a display name ending with 'Step 1'"
                .And(() => results[0].DisplayName.Should().EndWith("Step 1"));

            "And the second result should have a display name ending with 'Step 2'"
                .And(() => results[1].DisplayName.Should().EndWith("Step 2"));

            "And the third result should have a display name ending with 'Step 3'"
                .And(() => results[2].DisplayName.Should().EndWith("Step 3"));
        }

#if !V2
        [Scenario]
        public static void OrderingStepsByDisplayName()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given two steps named 'z' and 'y'"
                .f(() => feature = typeof(TwoStepsNamedZandY));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "And I sort the results by their display name"
                .f(() => results = results.OrderBy(result => result.DisplayName).ToArray());

            "Then the first result should have a display name ending with 'z'"
                .f(() => results[0].DisplayName.Should().EndWith("z"));

            "And the second result should have a display name ending with 'y'"
                .f(() => results[1].DisplayName.Should().EndWith("y"));
        }
#endif

        [Scenario]
        public static void ScenarioWithThreePassingSteps()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with three passing steps"
                .Given(() => feature = typeof(FeatureWithAScenarioWithThreePassingSteps));

            "When I run the scenarios"
                .When(() => results = feature.RunScenarios());

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

            "When I run the scenarios"
                .When(() => exception = Record.Exception(() => results = feature.RunScenarios()));

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

            "When I run the scenarios"
                .When(() => exception = Record.Exception(() => results = feature.RunScenarios()));

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

            "When I run the scenarios"
                .When(() => results = feature.RunScenarios());

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

#if !V2
        private static class TwoStepsNamedZandY
        {
            [Scenario]
            public static void Scenario()
            {
                "z"
                    .f(() => { });

                "y"
                    .f(() => { });
            }
        }
#endif

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
