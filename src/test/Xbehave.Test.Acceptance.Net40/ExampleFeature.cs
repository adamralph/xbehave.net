// <copyright file="ExampleFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;
#if !V2
    using Xunit.Extensions;
#endif
    using Xunit.Sdk;

    // In order to save time
    // As a developer
    // I want to write a single scenario using many examples
    public class ExampleFeature : Feature
    {
        [Scenario]
        public void Examples()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a feature with a scenario with examples"
                .f(() => feature = typeof(SingleStepAndThreeExamples));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<ITestPassed>(
                    results.ToDisplayString("the results should all be passes")));

            "And there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And the display name of one result should contain '(x: 1, y: 2, sum: 3)'"
                .f(() => results.Should().ContainSingle(result =>
                    result.Test.DisplayName.Contains("(x: 1, y: 2, sum: 3)")));

            "And the display name of one result should contain '(x: 10, y: 20, sum: 30)'"
                .f(() => results.Should().ContainSingle(result =>
                    result.Test.DisplayName.Contains("(x: 10, y: 20, sum: 30)")));

            "And the display name of one result should contain '(x: 100, y: 200, sum: 300)'"
                .f(() => results.Should().ContainSingle(result =>
                    result.Test.DisplayName.Contains("(x: 100, y: 200, sum: 300)")));
        }

        [Scenario]
        public void OrderingStepsByDisplayName()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given three steps named 'z' and 'y' each rendering two examples of 0 and 1"
                .f(() => feature = typeof(TenStepsNamedAlphabeticallyBackwardsAndTwoIdenticalExamples));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "And I sort the results by their display name"
                .f(() => results = results.OrderBy(result => result.Test.DisplayName).ToArray());

            "Then a concatenation of the last character of each result display names should be 'zyxwvutsrqzyxwvutsrq'"
                .f(() => new string(results.Select(result => result.Test.DisplayName.Last()).ToArray())
                    .Should().Be("zyxwvutsrqzyxwvutsrq"));
        }

        [Scenario]
        public void ExamplesWithMissingValues(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with three parameters, a single step and three examples each with one value"
                .f(() => feature = typeof(ScenarioWithThreeParametersASingleStepAndThreeExamplesEachWithOneValue));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<ITestPassed>());

            "And each result should contain the example value"
                .f(() =>
                {
                    foreach (var result in results)
                    {
                        result.Test.DisplayName.Should().Contain("example:");
                    }
                });

            "And each result should not contain the missing values"
                .f(() =>
                {
                    foreach (var result in results)
                    {
                        result.Test.DisplayName.Should().NotContain("missing1:");
                        result.Test.DisplayName.Should().NotContain("missing2:");
                    }
                });
        }

        [Scenario]
        public void ExamplesWithTwoMissingResolvableGenericArguments(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with a single step and examples with one argument missing"
                .f(() => feature = typeof(SingleStepAndThreeExamplesWithMissingResolvableGenericArguments));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

        [Scenario]
        public void GenericScenario(Type feature, ITestResultMessage[] results)
        {
            @"Given a feature with a scenario with one step, five type parameters and three examples each containing
an Int32 value for an argument defined using the first type parameter,
an Int64 value for an argument defined using the second type parameter,
an String value for an argument defined using the third type parameter,
an Int32 value for an argument defined using the fourth type parameter,
an Int64 value for another argument defined using the fourth type parameter and
an null value for an argument defined using the fifth type parameter"
                .f(() => feature = typeof(GenericScenarioFeature));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "And the display name of each result should contain \"<Int32, Int64, String, Object, Object>\""
                .f(() =>
                {
                    foreach (var result in results)
                    {
                        result.Test.DisplayName.Should().Contain("<Int32, Int64, String, Object, Object>");
                    }
                });
        }

        [Scenario]
        public void FormattedSteps(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{0}}, {{1}} and {{2}}\""
                .f(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndAFormattedStep));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .f(() => results.Count().Should().Be(1));

            "And the display name of the result should end with \"Given 1, 2 and 3\""
                .f(() => results.Single().Test.DisplayName.Should().EndWith("Given 1, 2 and 3"));
        }

        [Scenario]
        public void FormattedStepsWithNullValues(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{0}}, {{1}} and {{2}}\""
                .f(() => feature = typeof(FeatureWithAScenarioWithNullExampleValuesAndAFormattedStep));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .f(() => results.Count().Should().Be(1));

            "And the display name of the result should end with \"Given null, null and null\""
                .f(() => results.Single().Test.DisplayName.Should().EndWith("Given null, null and null"));
        }

        [Scenario]
        public void BadlyFormattedSteps(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{3}}, {{4}} and {{5}}\""
                .f(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .f(() => results.Count().Should().Be(1));

            "And the result should not be a pass"
                .f(() => results.Single().Should().BeAssignableTo<ITestPassed>());

            "And the display name of the result should end with \"Given {{3}}, {{4}} and {{5}}\""
                .f(() => results.Single().Test.DisplayName.Should().EndWith("Given {3}, {4} and {5}"));
        }

        [Scenario]
        public void InvalidExamples(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with scenarios with invalid examples"
                .f(() => feature = typeof(FeatureWithTwoScenariosWithInvalidExamples));

            "When I run the scenarios"
                .f(() => exception = Record.Exception(() =>
                    results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown"
                .f(() => exception.Should().BeNull());

            "And there should be 2 results"
                .f(() => results.Count().Should().Be(2));

            "And each result should be a failure"
                .f(() => results.Should().ContainItemsAssignableTo<ITestFailed>());
        }

        [Scenario]
        public void DiscoveryFailure(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with two scenarios with examples which throw errors"
                .f(() => feature = typeof(FeatureWithTwoScenariosWithExamplesWhichThrowErrors));

            "When I run the scenarios"
                .f(() => exception = Record.Exception(() =>
                    results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown"
                .f(() => exception.Should().BeNull());

            "And there should be 2 results"
                .f(() => results.Count().Should().Be(2));

            "And each result should be a failure"
                .f(() => results.Should().ContainItemsAssignableTo<ITestFailed>());
        }

#if V2
        [Scenario]
        public void ExampleValueDisposalFailure(Type feature, Exception exception, ITestCaseCleanupFailure[] failures)
        {
            "Given a feature with two scenarios with examples with values which throw exceptions when disposed"
                .f(() => feature = typeof(FeatureWithTwoScenariosWithExamplesWithValuesWhichThrowErrorsWhenDisposed));

            "When I run the scenarios"
                .f(() => exception = Record.Exception(() => failures = this.Run<ITestCaseCleanupFailure>(feature)));

            "Then no exception should be thrown"
                .f(() => exception.Should().BeNull());

            "And there should be 2 test case clean up failures"
                .f(() => failures.Count().Should().Be(2));
        }
#endif

#if !V2
        [Scenario]
        public void OmissionOfArgumentsFromScenarioNames(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with a single step and examples and omission of arguments from scenario names"
                .f(() => feature = typeof(ASingleStepAndExamplesWithOmissionOfArgumentsFromScenarioNames));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then the display name of no result should contain '(x: 1, y: 2, z: 3)'"
                .f(() => results.Should().NotContain(result => result.Test.DisplayName.Contains("(x: 1, y: 2, z: 3)")));

            "And the display name of no result should contain '(x: 3, y: 4, z: 5)'"
                .f(() => results.Should().NotContain(result => result.Test.DisplayName.Contains("(x: 3, y: 4, z: 5)")));

            "And the display name of no result should contain '(x: 5, y: 6, z: 7)'"
                .f(() => results.Should().NotContain(result => result.Test.DisplayName.Contains("(x: 5, y: 6, z: 7)")));
        }
#endif

#if V2
        public class BadExampleAttribute : MemberDataAttributeBase
        {
            public BadExampleAttribute()
                : base("Dummy", new object[0])
            {
            }

            protected override object[] ConvertDataItem(MethodInfo testMethod, object item)
            {
                throw new NotImplementedException();
            }
        }

        public class BadValuesExampleAttribute : DataAttribute
        {
            public BadValuesExampleAttribute()
            {
            }

            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                yield return new object[] { new BadDisposable() };
            }
        }

        public class BadDisposable : IDisposable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
#else
        public class BadExampleAttribute : PropertyDataAttribute
        {
            public BadExampleAttribute()
                : base("Dummy")
            {
            }

            public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest, Type[] parameterTypes)
            {
                throw new NotImplementedException();
            }
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
                    .f(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (x + y).Should().Be(sum);
                        previousSum = sum;
                    });
            }
        }

        private static class TenStepsNamedAlphabeticallyBackwardsAndTwoIdenticalExamples
        {
            [Scenario]
            [Example(0)]
            [Example(0)]
            public static void Scenario(int x)
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

        private static class ScenarioWithThreeParametersASingleStepAndThreeExamplesEachWithOneValue
        {
            private static int previousExample;

            [Scenario]
            [Example(1)]
            [Example(2)]
            [Example(3)]
            public static void Scenario(int example, int missing1, object missing2)
            {
                "Then distinct examples are passed with the default values for missing arguments"
                    .f(() =>
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
                    .f(() =>
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
                    .f(() => { });
            }
        }

        private static class FeatureWithAScenarioWithExampleValuesAndAFormattedStep
        {
            [Scenario]
            [Example(1, 2, 3)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {0}, {1} and {2}"
                    .f(() => { });
            }
        }

        private static class FeatureWithAScenarioWithNullExampleValuesAndAFormattedStep
        {
            [Scenario]
            [Example(null, null, null)]
            public static void Scenario(object x, object y, object z)
            {
                "Given {0}, {1} and {2}"
                    .f(() => { });
            }
        }

        private static class FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep
        {
            [Scenario]
            [Example(1, 2, 3)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {3}, {4} and {5}"
                    .f(() => { });
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

        private static class FeatureWithTwoScenariosWithExamplesWhichThrowErrors
        {
            [Scenario]
            [BadExample]
            public static void Scenario1(int i)
            {
            }

            [Scenario]
            [BadExample]
            public static void Scenario2(int i)
            {
            }
        }

#if V2
        private static class FeatureWithTwoScenariosWithExamplesWithValuesWhichThrowErrorsWhenDisposed
        {
            [Scenario]
            [BadValuesExample]
            public static void Scenario1(BadDisposable obj)
            {
            }

            [Scenario]
            [BadValuesExample]
            public static void Scenario2(BadDisposable obj)
            {
            }
        }
#endif

#if !V2
        [OmitArgumentsFromScenarioNames(true)]
        private static class ASingleStepAndExamplesWithOmissionOfArgumentsFromScenarioNames
        {
            [Scenario]
            [Example(1, 2, 3)]
            [Example(3, 4, 5)]
            [Example(5, 6, 7)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {0}, {1} and {2}"
                    .f(() => { });
            }
        }
#endif
    }
}
