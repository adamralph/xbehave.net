// <copyright file="ExampleFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;
    using Xunit.Sdk;

    // In order to save time
    // As a developer
    // I want to write a single scenario using many examples
    public static class ExampleFeature
    {
        [Scenario]
        public static void Examples()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with examples"
                .f(() => feature = typeof(SingleStepAndThreeExamples));

            "When the test runner runs the feature"
                .f(() => results = feature.RunScenarios().ToArray());

            "Then each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>(
                    results.ToDisplayString("the results should all be passes")));

            "And there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And the display name of one result should contain '(x: 1, y: 2, sum: 3)'"
                .f(() => results.Should().ContainSingle(result =>
                    result.DisplayName.Contains("(x: 1, y: 2, sum: 3)")));

            "And the display name of one result should contain '(x: 10, y: 20, sum: 30)'"
                .f(() => results.Should().ContainSingle(result =>
                    result.DisplayName.Contains("(x: 10, y: 20, sum: 30)")));

            "And the display name of one result should contain '(x: 100, y: 200, sum: 300)'"
                .f(() => results.Should().ContainSingle(result =>
                    result.DisplayName.Contains("(x: 100, y: 200, sum: 300)")));
        }

        [Scenario]
        public static void ExamplesWithTwoMissingArguments(Type feature, Result[] results)
        {
            "Given a feature with a scenario with a single step and examples with one argument missing"
                .Given(() => feature = typeof(SingleStepAndThreeExamplesWithTwoMissingArguments));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray());

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>());

            "And each result should contain the example value"
                .f(() =>
                {
                    foreach (var result in results)
                    {
                        result.DisplayName.Should().Contain("example:");
                    }
                });

            "And each result should not contain the missing values"
                .f(() =>
                {
                    foreach (var result in results)
                    {
                        result.DisplayName.Should().NotContain("missing1:");
                        result.DisplayName.Should().NotContain("missing2:");
                    }
                });
        }

        [Scenario]
        public static void ExamplesWithTwoMissingResolvableGenericArguments(Type feature, Result[] results)
        {
            "Given a feature with a scenario with a single step and examples with one argument missing"
                .Given(() => feature = typeof(SingleStepAndThreeExamplesWithMissingResolvableGenericArguments));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray());

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>());
        }

        [Scenario]
        public static void GenericScenario(Type feature, Result[] results)
        {
            @"Given a feature with a scenario with one step, five type parameters and three examples each containing
an Int32 value for an argument defined using the first type parameter,
an Int64 value for an argument defined using the second type parameter,
an String value for an argument defined using the third type parameter,
an Int32 value for an argument defined using the fourth type parameter,
an Int64 value for another argument defined using the fourth type parameter and
an null value for an argument defined using the fifth type parameter"
                .f(() => feature = typeof(GenericScenarioFeature));

            "When the test runner runs the feature"
                .f(() => results = feature.RunScenarios().ToArray());

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And the display name of each result should contain \"<Int32, Int64, String, Object, Object>\""
                .f(() =>
                {
                    foreach (var result in results)
                    {
                        result.DisplayName.Should().Contain("<Int32, Int64, String, Object, Object>");
                    }
                });
        }

        [Scenario]
        public static void FormattedSteps(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{0}}, {{1}} and {{2}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndAFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the display name of the result should end with \"Given 1, 2 and 3\""
                .And(() => results.Single().DisplayName.Should().EndWith("Given 1, 2 and 3"));
        }

        [Scenario]
        public static void FormattedStepsWithNullValues(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{0}}, {{1}} and {{2}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithNullExampleValuesAndAFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the display name of the result should end with \"Given null, null and null\""
                .And(() => results.Single().DisplayName.Should().EndWith("Given null, null and null"));
        }

        [Scenario]
        public static void BadlyFormattedSteps(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{3}}, {{4}} and {{5}}\""
                .Given(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should not be a pass"
                .And(() => results.Single().Should().BeOfType<Pass>());

            "And the display name of the result should end with \"Given {{3}}, {{4}} and {{5}}\""
                .And(() => results.Single().DisplayName.Should().EndWith("Given {3}, {4} and {5}"));
        }

        [Scenario]
        public static void InvalidExamples(Type feature, Exception exception, IEnumerable<Result> results)
        {
            "Given a feature with scenarios with invalid examples"
                .Given(() => feature = typeof(FeatureWithTwoScenariosWithInvalidExamples));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = feature.RunScenarios().ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And there should be 2 results"
                .And(() => results.Count().Should().Be(2));

            "And each result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<Fail>());
        }

#if !V2
        [Scenario]
        public static void OmissionOfArgumentsFromScenarioNames(Type feature, IEnumerable<Result> results)
        {
            "Given a feature with a scenario with a single step and examples and omission of arguments from scenario names"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndExamplesWithOmissionOfArgumentsFromScenarioNames));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray());

            "Then the display name of no result should contain '(x: 1, y: 2, z: 3)'"
                .Then(() => results.Should().NotContain(result => result.DisplayName.Contains("(x: 1, y: 2, z: 3)")));

            "And the display name of no result should contain '(x: 3, y: 4, z: 5)'"
                .And(() => results.Should().NotContain(result => result.DisplayName.Contains("(x: 3, y: 4, z: 5)")));

            "And the display name of no result should contain '(x: 5, y: 6, z: 7)'"
                .And(() => results.Should().NotContain(result => result.DisplayName.Contains("(x: 5, y: 6, z: 7)")));
        }
#endif

        private static class SingleStepAndThreeExamples
        {
            private static int previousSum;

            [Scenario]
            [Example(1, 2, 3)]
            [Example(10, 20, 30)]
            [Example(100, 200, 300)]
            public static void Scenario(int x, int y, int sum)
            {
                "Then as a distinct example the sum of {0} and {1} is {2}"
                    .Given(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (x + y).Should().Be(sum);
                        previousSum = sum;
                    });
            }
        }

        private static class SingleStepAndThreeExamplesWithTwoMissingArguments
        {
            private static int previousExample;

            [Scenario]
            [Example(1)]
            [Example(2)]
            [Example(3)]
            public static void Scenario(int example, int missing1, object missing2)
            {
                "Then distinct examples are passed with the default values for missing arguments"
                    .Given(() =>
                    {
                        example.Should().NotBe(previousExample);
                        missing1.Should().Be(default(int));
                        missing2.Should().Be(default(object));
                        previousExample = example;
                    });
            }
        }

        private static class SingleStepAndThreeExamplesWithMissingResolvableGenericArguments
        {
            private static object previousExample1;
            private static object previousExample2;

            [Scenario]
            [Example(1, "a")]
            [Example(3, "b")]
            [Example(5, "c")]
            public static void Scenario<T1, T2>(T1 example1, T2 example2, T1 missing1, T2 missing2)
            {
                "Then distinct examples are passed with the default values for missing arguments"
                    .Given(() =>
                    {
                        example1.Should().NotBe(previousExample1);
                        example2.Should().NotBe(previousExample2);
                        missing1.Should().Be(default(T1));
                        missing2.Should().Be(default(T2));
                        previousExample1 = example1;
                        previousExample2 = example2;
                    });
            }
        }

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

#if !V2
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
#endif
    }
}
