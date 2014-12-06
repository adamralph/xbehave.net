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
        public static void ScenarioWithTwoPassingStepsAndOneFailingStepYieldsTwoPassesAndOneFail()
        {
            // arrange
            var feature = typeof(FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep);

            // act
            var results = feature.RunScenarios();

            // assert
            results.Length.Should().Be(3);
            results.Take(2).Should().ContainItemsAssignableTo<Pass>();
            results.Skip(2).Should().ContainItemsAssignableTo<Fail>();
        }

        [Scenario]
        public static void ScenarioWithThreeSteps()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with three steps"
                .f(() => feature = typeof(FeatureWithAScenarioWithThreeSteps));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And the first result should have a display name ending with 'Step 1'"
                .f(() => results[0].DisplayName.Should().EndWith("Step 1"));

            "And the second result should have a display name ending with 'Step 2'"
                .f(() => results[1].DisplayName.Should().EndWith("Step 2"));

            "And the third result should have a display name ending with 'Step 3'"
                .f(() => results[2].DisplayName.Should().EndWith("Step 3"));
        }

        [Scenario]
        public static void OrderingStepsByDisplayName()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given two steps named 'z' and 'y'"
                .f(() => feature = typeof(TenStepsNamedAlphabeticallyBackwards));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "And I sort the results by their display name"
                .f(() => results = results.OrderBy(result => result.DisplayName).ToArray());

            "Then a concatenation of the last character of each result display names should be 'zyxwvutsrq'"
                .f(() => new string(results.Select(result => result.DisplayName.Last()).ToArray())
                    .Should().Be("zyxwvutsrq"));
        }

        [Scenario]
        public static void ScenarioWithTwoPassingStepsAndOneFailingStep()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with two passing steps and one failing step"
                .f(() => feature = typeof(FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And the first two results should be passes"
                .f(() => results.Take(2).Should().ContainItemsAssignableTo<Pass>());

            "And the third result should be a fail"
                .f(() => results.Skip(2).Should().ContainItemsAssignableTo<Fail>());
        }

        [Scenario]
        public static void ScenarioBodyThrowsAnException()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(Result[]);

            "Given a feature with a scenario body which throws an exception"
                .f(() => feature = typeof(FeatureWithAScenarioBodyWhichThrowsAnException));

            "When I run the scenarios"
                .f(() => exception = Record.Exception(() => results = feature.RunScenarios()));

            "Then no exception should be thrown"
                .f(() => exception.Should().BeNull());

            "And the results should not be empty"
                .f(() => results.Should().NotBeEmpty());

            "And each result should be a failure"
                .f(() => results.Should().ContainItemsAssignableTo<Fail>());
        }

        [Scenario]
        public static void FeatureCannotBeConstructed()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(Result[]);

            "Given a feature with a non-static scenario but no default constructor"
                .f(() => feature = typeof(FeatureWithANonStaticScenarioButNoDefaultConstructor));

            "When I run the scenarios"
                .f(() => exception = Record.Exception(() => results = feature.RunScenarios()));

            "Then no exception should be thrown"
                .f(() => exception.Should().BeNull());

            "And the results should not be empty"
                .f(() => results.Should().NotBeEmpty());

            "And each result should be a failure"
                .f(() => results.Should().ContainItemsAssignableTo<Fail>());
        }

        [Scenario]
        public static void FailingStep()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a failing step followed by passing steps"
                .f(() => feature = typeof(FeatureWithAFailingStepFollowedByTwoPassingSteps));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then each result should be a failure"
                .f(() => results.Should().ContainItemsAssignableTo<Fail>());

            "And each subsequent result message should indicate that the first step failed"
                .f(() =>
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

        private static class TenStepsNamedAlphabeticallyBackwards
        {
            [Scenario]
            public static void Scenario()
            {
                "z"
                    .f(() => { });

                "y"
                    .f(() => { });

                "x"
                    .f(() => { });

                "w"
                    .f(() => { });

                "v"
                    .f(() => { });

                "u"
                    .f(() => { });

                "t"
                    .f(() => { });

                "s"
                    .f(() => { });

                "r"
                    .f(() => { });

                "q"
                    .f(() => { });
            }
        }

        private static class FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep
        {
            [Scenario]
            public static void Scenario()
            {
                var i = 0;

                "Given 1"
                    .f(() => i = 1);

                "When I add 1"
                    .f(() => i += 1);

                "Then I have 3"
                    .f(() => i.Should().Be(3));
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
                    .f(() =>
                    {
                        throw new NotImplementedException();
                    });

                "When something happens"
                    .f(() => { });

                "Then there is an outcome"
                    .f(() => { });
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
                    .f(() => { });
            }
        }
    }
}
