// <copyright file="ExampleFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to save time
    // As a developer
    // I want to write a single scenario using many examples
    public static class ExampleFeature
    {
        private static readonly ConcurrentStack<object[]> ArgumentLists = new ConcurrentStack<object[]>();

        [Scenario]
        public static void Examples()
        {
            var feature = default(Type);

            "Given a feature with a scenario with examples"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndExamples));

            "When the test runner runs the feature"
                .When(() => TestRunner.Run(feature));

            "Then the scenario should be executed once for each example with the values from that example passed as arguments"
                .Then(() =>
                {
                    ArgumentLists.Select(arguments => arguments.Cast<int>()).OrderBy(x => x, new EnumerableComparer<int>())
                        .SequenceEqual(
                            Reflector.Wrap(feature.GetMethod("Scenario")).GetCustomAttributes(typeof(ExampleAttribute)).Select(x => x.GetInstance<ExampleAttribute>())
                                .Select(example => example.DataValues.Cast<int>()).OrderBy(x => x, new EnumerableComparer<int>()),
                            new EnumerableEqualityComparer<int>()).Should().BeTrue();
                });
        }

        [Scenario]
        public static void GenericScenario()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a generic scenario with four type parameters and examples containing an Int32 value, an Int64 value and a String value"
                .Given(() => feature = typeof(FeatureWithAGenericScenarioWithExamplesContainingAnInt32ValueAnInt64ValueAndAStringValue));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the display name of each result should contain \"<Int32, Int64, String, Object>\""
                .And(() => results.Should().OnlyContain(result => result.DisplayName.Contains("<Int32, Int64, String, Object>")));
        }

        [Scenario]
        public static void FormattedSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{0}}, {{1}} and {{2}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndAFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the display name of each result should contain \"Given 1, 2 and 3\""
                .And(() => results.Should().OnlyContain(result => result.DisplayName.Contains("Given 1, 2 and 3")));
        }

        [Scenario]
        public static void BadlyFormattedSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{3}}, {{4}} and {{5}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .And(() => results.Should().NotContain(result => result is FailedResult));

            "And the display name of each result should end with \"Given {{3}}, {{4}} and {{5}}\""
                .And(() => results.Should().OnlyContain(result => result.DisplayName.EndsWith("Given {3}, {4} and {5}")));
        }

        private static class FeatureWithAScenarioWithASingleStepAndExamples
        {
            [Scenario]
            [Example(1, 2, 3)]
            [Example(3, 4, 5)]
            [Example(5, 6, 7)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {0}, {1} and {2}"
                    .Given(() => ArgumentLists.Push(new object[] { x, y, z }));
            }
        }

        private static class FeatureWithAGenericScenarioWithExamplesContainingAnInt32ValueAnInt64ValueAndAStringValue
        {
            [Scenario]
            [Example(1, 2L, "a")]
            [Example(3, 4L, "a")]
            [Example(5, 6L, "a")]
            public static void Scenario<T1, T2, T3, T4>(T1 x, T2 y, T3 z)
            {
                "Given"
                    .Given(() => { });

                "When"
                    .When(() => { });

                "Then"
                    .Then(() => { });
            }
        }

        private static class FeatureWithAScenarioWithExampleValuesAndAFormattedStep
        {
            [Scenario]
            [Example(1, 2, 3)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {0}, {1} and {2}"
                    .Given(() => { });
            }
        }

        private static class FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep
        {
            [Scenario]
            [Example(1, 2, 3)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {3}, {4} and {5}"
                    .Given(() => { });
            }
        }
    }
}
