// <copyright file="ExampleFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
#if NET40 || NET45
    using System.Collections.Concurrent;
#endif
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Features.Infrastructure;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;
    using Xunit.Sdk;

    // In order to save time
    // As a developer
    // I want to write a single scenario using many examples
    public static class ExampleFeature
    {
#if NET40 || NET45
        private static readonly ConcurrentStack<object[]> ArgumentLists = new ConcurrentStack<object[]>();
#endif
#if NET40 || NET45

        [Scenario]
        public static void Examples(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with a single step and examples"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndExamples))
                .Teardown(() => ArgumentLists.Clear());

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature));

            "Then the scenario should be executed once for each example with the values from that example passed as arguments"
                .Then(() => ArgumentLists
                    .Select(arguments => arguments.Cast<int>().ToArray())
                    .OrderBy(x => x, new EnumerableComparer<int>())
                    .SequenceEqual(
                        Reflector.Wrap(feature.GetMethods().First()).GetCustomAttributes(typeof(ExampleAttribute))
                            .Select(x => x.GetExampleValues().Cast<int>().ToArray())
                            .OrderBy(x => x, new EnumerableComparer<int>()),
                        new EnumerableEqualityComparer<int>())
                    .Should().BeTrue());

            "And the display name of one result should contain '(x: 1, y: 2, z: 3)'"
                .And(() => results.Should().ContainSingle(result => result.DisplayName.Contains("(x: 1, y: 2, z: 3)")));

            "And the display name of one result should contain '(x: 3, y: 4, z: 5)'"
                .And(() => results.Should().ContainSingle(result => result.DisplayName.Contains("(x: 3, y: 4, z: 5)")));

            "And the display name of one result should contain '(x: 5, y: 6, z: 7)'"
                .And(() => results.Should().ContainSingle(result => result.DisplayName.Contains("(x: 5, y: 6, z: 7)")));
        }

        [Scenario]
        public static void ExamplesWithTwoMissingArguments(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with a single step and examples with one argument missing"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndExamplesWithTwoMissingArguments))
                .Teardown(() => ArgumentLists.Clear());

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then each result should be a success"
                .Then(() => results.Should().ContainItemsAssignableTo<Pass>());

            ("Then the scenario should be executed once for each example with " +
                "the values from that example passed as the first two arguments and " +
                "default values passed as the remaining two arguments")
                .Then(() => ArgumentLists
                    .Select(arguments => arguments.Cast<int>().ToArray())
                    .OrderBy(x => x, new EnumerableComparer<int>())
                    .SequenceEqual(
                        Reflector.Wrap(feature.GetMethods().First()).GetCustomAttributes(typeof(ExampleAttribute))
                            .Select(x => x.GetExampleValues().Cast<int>()
                                .Concat(new[]
                                    {
                                        default(int),
                                        default(int),
                                    })
                                .ToArray())
                            .OrderBy(x => x, new EnumerableComparer<int>()),
                        new EnumerableEqualityComparer<int>())
                    .Should().BeTrue());
        }

        [Scenario]
        public static void ExamplesWithTwoMissingResolvableGenericArguments(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with a single step and examples with one argument missing"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndExamplesWithTwoMissingResolvableGenericArguments))
                .Teardown(() => ArgumentLists.Clear());

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then each result should be a success"
                .Then(() => results.Should().ContainItemsAssignableTo<Pass>());

            ("Then the scenario should be executed once for each example with " +
                "the values from that example passed as the first two arguments and " +
                "default values passed as the remaining two arguments")
                .Then(() => ArgumentLists
                    .Select(arguments => arguments.Cast<int>().ToArray())
                    .OrderBy(x => x, new EnumerableComparer<int>())
                    .SequenceEqual(
                        Reflector.Wrap(feature.GetMethods().First()).GetCustomAttributes(typeof(ExampleAttribute))
                            .Select(x => x.GetExampleValues().Cast<int>()
                                .Concat(new[]
                                    {
                                        default(int),
                                        default(int),
                                    })
                                .ToArray())
                            .OrderBy(x => x, new EnumerableComparer<int>()),
                        new EnumerableEqualityComparer<int>())
                    .Should().BeTrue());
        }
#endif

        [Scenario]
        public static void GenericScenario(Type feature, IEnumerable<Result> results)
        {
            @"Given a feature with a generic scenario with five type parameters and examples containing
an Int32 value for the first type parameter,
an Int64 value for the second type parameter,
an String value for the third type parameter,
an Int32 value for the fourth type parameter,
an Int64 value for the fourth type parameter and
an null value for the fifth type parameter"
                .Given(() => feature = typeof(GenericScenarioFeature));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the display name of each result should contain \"<Int32, Int64, String, Object, Object>\""
                .And(() => results.Should().OnlyContain(result => result.DisplayName.Contains("<Int32, Int64, String, Object, Object>")));
        }

        [Scenario]
        public static void FormattedSteps(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{0}}, {{1}} and {{2}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndAFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the display name of each result should contain \"Given 1, 2 and 3\""
                .And(() => results.Should().OnlyContain(result => result.DisplayName.EndsWith("Given 1, 2 and 3")));
        }

        [Scenario]
        public static void FormattedStepsWithNullValues(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{0}}, {{1}} and {{2}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithNullExampleValuesAndAFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the display name of each result should contain \"Given null, null and null\""
                .And(() => results.Should().OnlyContain(result => result.DisplayName.EndsWith("Given null, null and null")));
        }

        [Scenario]
        public static void BadlyFormattedSteps(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{3}}, {{4}} and {{5}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .And(() => results.Should().NotContain(result => result is Fail));

            "And the display name of each result should end with \"Given {{3}}, {{4}} and {{5}}\""
                .And(() => results.Should().OnlyContain(result => result.DisplayName.EndsWith("Given {3}, {4} and {5}")));
        }

        [Scenario]
        public static void InvalidExamples(Type feature, Exception exception, IEnumerable<Result> results)
        {
            "Given a feature with scenarios with invalid examples"
                .Given(() => feature = typeof(FeatureWithTwoScenariosWithInvalidExamples));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And there should be 2 results"
                .And(() => results.Count().Should().Be(2));

            "And each result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<Fail>());
        }

        [Scenario]
        public static void OmissionOfArgumentsFromScenarioNames(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with a single step and examples and omission of arguments from scenario names"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndExamplesWithOmissionOfArgumentsFromScenarioNames));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the display name of no result should contain '(x: 1, y: 2, z: 3)'"
                .Then(() => results.Should().NotContain(result => result.DisplayName.Contains("(x: 1, y: 2, z: 3)")));

            "And the display name of no result should contain '(x: 3, y: 4, z: 5)'"
                .And(() => results.Should().NotContain(result => result.DisplayName.Contains("(x: 3, y: 4, z: 5)")));

            "And the display name of no result should contain '(x: 5, y: 6, z: 7)'"
                .And(() => results.Should().NotContain(result => result.DisplayName.Contains("(x: 5, y: 6, z: 7)")));
        }

#if NET40 || NET45
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

        private static class FeatureWithAScenarioWithASingleStepAndExamplesWithTwoMissingArguments
        {
            [Scenario]
            [Example(1, 2)]
            [Example(3, 4)]
            [Example(5, 6)]
            public static void Scenario(int w, int x, int y, int z)
            {
                "Given {0}, {1}, {2} and {3}"
                    .Given(() => ArgumentLists.Push(new object[] { w, x, y, z }));
            }
        }

        private static class FeatureWithAScenarioWithASingleStepAndExamplesWithTwoMissingResolvableGenericArguments
        {
            [Scenario]
            [Example(1, 2)]
            [Example(3, 4)]
            [Example(5, 6)]
            public static void Scenario<T1, T2>(T1 w, T2 x, T1 y, T2 z)
            {
                "Given {0}, {1}, {2} and {3}"
                    .Given(() => ArgumentLists.Push(new object[] { w, x, y, z }));
            }
        }
#endif

        private static class GenericScenarioFeature
        {
            [Scenario]
            [Example(1, 2L, "a", 7, 7L, null)]
            [Example(3, 4L, "a", 8, 8L, null)]
            [Example(5, 6L, "a", 9, 8L, null)]
            public static void Scenario<T1, T2, T3, T4, T5>(T1 a, T2 b, T3 c, T4 d, T4 e, T5 f)
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

        private static class FeatureWithAScenarioWithNullExampleValuesAndAFormattedStep
        {
            [Scenario]
            [Example(null, null, null)]
            public static void Scenario(object x, object y, object z)
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

        private static class FeatureWithTwoScenariosWithInvalidExamples
        {
            [Scenario]
            [Example("a")]
            public static void Scenario2(int i)
            {
            }

            [Scenario]
            [Example(1, 2)]
            public static void Scenario3(int i)
            {
            }
        }

        [OmitArgumentsFromScenarioNames(true)]
        private static class FeatureWithAScenarioWithASingleStepAndExamplesWithOmissionOfArgumentsFromScenarioNames
        {
            [Scenario]
            [Example(1, 2, 3)]
            [Example(3, 4, 5)]
            [Example(5, 6, 7)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {0}, {1} and {2}"
                    .Given(() => { });
            }
        }
    }
}
