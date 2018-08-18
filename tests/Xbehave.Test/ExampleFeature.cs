namespace Xbehave.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;
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
                .x(() => feature = typeof(SingleStepAndThreeExamples));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>(
                    results.ToDisplayString("the results should all be passes")));

            "And there should be three results"
                .x(() => results.Length.Should().Be(3));

            "And the display name of one result should contain '(x: 1, y: 2, sum: 3)'"
                .x(() => results.Should().ContainSingle(result =>
                    result.Test.DisplayName.Contains("(x: 1, y: 2, sum: 3)")));

            "And the display name of one result should contain '(x: 10, y: 20, sum: 30)'"
                .x(() => results.Should().ContainSingle(result =>
                    result.Test.DisplayName.Contains("(x: 10, y: 20, sum: 30)")));

            "And the display name of one result should contain '(x: 100, y: 200, sum: 300)'"
                .x(() => results.Should().ContainSingle(result =>
                    result.Test.DisplayName.Contains("(x: 100, y: 200, sum: 300)")));
        }

        [Scenario]
        public void SkippedExamples()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given two examples with a problematic one skipped"
                .x(() => feature = typeof(TwoExamplesWithAProblematicOneSkipped));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be two results"
                .x(() => results.Length.Should().Be(2));

            "And one result should be a pass"
                .x(() => results.OfType<ITestPassed>().Count().Should().Be(1));

            "And there should be no failures"
                .x(() => results.OfType<ITestFailed>().Count().Should().Be(0));

            "And one result should be a skip"
                .x(() => results.OfType<ITestSkipped>().Count().Should().Be(1));
        }

        [Scenario]
        public void ExamplesWithArrays(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with array examples"
                .x(() => feature = typeof(ArrayExamples));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .x(() => results.Length.Should().Be(1));

            "And the display name of the result should contain '(words: [\"one\", \"two\"], numbers: [1, 2])'"
                .x(() => results.Single().Test.DisplayName.Should().Contain("(words: [\"one\", \"two\"], numbers: [1, 2])"));
        }

        [Scenario]
        public void ExamplesWithMissingValues(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with three parameters, a single step and three examples each with one value"
                .x(() => feature = typeof(ScenarioWithThreeParametersASingleStepAndThreeExamplesEachWithOneValue));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .x(() => results.Length.Should().Be(3));

            "And each result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());

            "And each result should contain the example value"
                .x(() =>
                {
                    foreach (var result in results)
                    {
                        result.Test.DisplayName.Should().Contain("example:");
                    }
                });

            "And each result should not contain the missing values"
                .x(() =>
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
                .x(() => feature = typeof(SingleStepAndThreeExamplesWithMissingResolvableGenericArguments));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .x(() => results.Length.Should().Be(3));

            "And each result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
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
                .x(() => feature = typeof(GenericScenarioFeature));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .x(() => results.Length.Should().Be(3));

            "And the display name of each result should contain \"<Int32, Int64, String, Object, Object>\""
                .x(() =>
                {
                    foreach (var result in results)
                    {
                        result.Test.DisplayName.Should().Contain("<Int32, Int64, String, Object, Object>");
                    }
                });
        }

        [Scenario]
        public void BadlyFormattedSteps(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with example values one two and three and a step with the format \"Given {{3}}, {{4}} and {{5}}\""
                .x(() => feature = typeof(FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .x(() => results.Count().Should().Be(1));

            "And the result should not be a pass"
                .x(() => results.Single().Should().BeAssignableTo<ITestPassed>());

            "And the display name of the result should end with \"Given {{3}}, {{4}} and {{5}}\""
                .x(() => results.Single().Test.DisplayName.Should().EndWith("Given {3}, {4} and {5}"));
        }

        [Scenario]
        public void InvalidExamples(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with scenarios with invalid examples"
                .x(() => feature = typeof(FeatureWithTwoScenariosWithInvalidExamples));

            "When I run the scenarios"
                .x(() => exception = Record.Exception(() =>
                    results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown"
                .x(() => exception.Should().BeNull());

            "And there should be 2 results"
                .x(() => results.Count().Should().Be(2));

            "And each result should be a failure"
                .x(() => results.Should().ContainItemsAssignableTo<ITestFailed>());
        }

        [Scenario]
        public void DiscoveryFailure(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with two scenarios with examples which throw errors"
                .x(() => feature = typeof(FeatureWithTwoScenariosWithExamplesWhichThrowErrors));

            "When I run the scenarios"
                .x(() => exception = Record.Exception(() => results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown"
                .x(() => exception.Should().BeNull());

            "And there should be 2 results"
                .x(() => results.Count().Should().Be(2));

            "And each result should be a failure"
                .x(() => results.Should().ContainItemsAssignableTo<ITestFailed>());
        }

        [Scenario]
        public void ExampleValueDisposalFailure(Type feature, Exception exception, ITestCaseCleanupFailure[] failures)
        {
            "Given a feature with two scenarios with examples with values which throw exceptions when disposed"
                .x(() => feature = typeof(FeatureWithTwoScenariosWithExamplesWithValuesWhichThrowErrorsWhenDisposed));

            "When I run the scenarios"
                .x(() => exception = Record.Exception(() => failures = this.Run<ITestCaseCleanupFailure>(feature)));

            "Then no exception should be thrown"
                .x(() => exception.Should().BeNull());

            "And there should be 2 test case clean up failures"
                .x(() => failures.Count().Should().Be(2));
        }

        [Scenario]
        public void DateTimeExampleValues(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given scenarios expecting DateTime example values"
                .x(() => feature = typeof(ScenariosExpectingDateTimeValues));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

        [Scenario]
        public void DateTimeOffsetExampleValues(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given scenarios expecting DateTimeOffset example values"
                .x(() => feature = typeof(ScenariosExpectingDateTimeOffsetValues));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

        [Scenario]
        public void GuidExampleValues(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given scenarios expecting Guid example values"
                .x(() => feature = typeof(ScenariosExpectingGuidValues));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

        public sealed class BadExampleAttribute : MemberDataAttributeBase
        {
            public BadExampleAttribute()
                : base("Dummy", new object[0])
            {
            }

            protected override object[] ConvertDataItem(MethodInfo testMethod, object item) =>
                throw new NotImplementedException();
        }

        public sealed class BadValuesExampleAttribute : DataAttribute
        {
            public BadValuesExampleAttribute()
            {
            }

            public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            {
                yield return new object[] { new BadDisposable() };
            }
        }

        public sealed class BadDisposable : IDisposable
        {
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Test")]
            public void Dispose() => throw new InvalidOperationException();
        }

        private static class SingleStepAndThreeExamples
        {
            private static int previousSum;

            [Scenario]
            [Example(1, 2, 3)]
            [Example(10, 20, 30)]
            [Example(100, 200, 300)]
            public static void Scenario(int x, int y, int sum) =>
                $"Then as a distinct example the sum of {x} and {y} is {sum}"
                    .x(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (x + y).Should().Be(sum);
                        previousSum = sum;
                    });
        }

        private static class TwoExamplesWithAProblematicOneSkipped
        {
            [Scenario]
            [Example(1)]
            [Example(2, Skip = "Because I can.")]
            public static void Scenario(int x) =>
                $"Given {x}"
                    .x(() =>
                    {
                        if (x == 2)
                        {
                            throw new Exception();
                        }
                    });
        }

        private static class ArrayExamples
        {
            [Scenario]
            [Example(new[] { "one", "two" }, new[] { 1, 2 })]
            public static void Scenario(string[] words, int[] numbers) =>
                "Given something"
                    .x(() => { });
        }

        private static class ScenarioWithThreeParametersASingleStepAndThreeExamplesEachWithOneValue
        {
            private static int previousExample;

            [Scenario]
            [Example(1)]
            [Example(2)]
            [Example(3)]
            public static void Scenario(int example, int missing1, object missing2) =>
                "Then distinct examples are passed with the default values for missing arguments"
                    .x(() =>
                    {
                        example.Should().NotBe(previousExample);
                        missing1.Should().Be(default);
                        missing2.Should().Be(default);
                        previousExample = example;
                    });
        }

        private static class SingleStepAndThreeExamplesWithMissingResolvableGenericArguments
        {
            private static object previousExample1;
            private static object previousExample2;

            [Scenario]
            [Example(1, "a")]
            [Example(3, "b")]
            [Example(5, "c")]
            public static void Scenario<T1, T2>(T1 example1, T2 example2, T1 missing1, T2 missing2) =>
                "Then distinct examples are passed with the default values for missing arguments"
                    .x(() =>
                    {
                        example1.Should().NotBe(previousExample1);
                        example2.Should().NotBe(previousExample2);
                        missing1.Should().Be(default(T1));
                        missing2.Should().Be(default(T2));
                        previousExample1 = example1;
                        previousExample2 = example2;
                    });
        }

        private static class GenericScenarioFeature
        {
            [Scenario]
            [Example(1, 2L, "a", 7, 7L, null)]
            [Example(3, 4L, "a", 8, 8L, null)]
            [Example(5, 6L, "a", 9, 8L, null)]
            public static void Scenario<T1, T2, T3, T4, T5>(T1 a, T2 b, T3 c, T4 d, T4 e, T5 f) =>
                "Given"
                    .x(() => { });
        }

        private static class FeatureWithAScenarioWithExampleValuesAndAFormattedStep
        {
            [Scenario]
            [Example(1, 2, 3)]
            public static void Scenario(int x, int y, int z) =>
                $"Given {x}, {y} and {z}"
                    .x(() => { });
        }

        private static class FeatureWithAScenarioWithNullExampleValuesAndAFormattedStep
        {
            [Scenario]
            [Example(null, null, null)]
            public static void Scenario(object x, object y, object z) =>
                $"Given {x}, {y} and {z}"
                    .x(() => { });
        }

        private static class FeatureWithAScenarioWithExampleValuesAndABadlyFormattedStep
        {
            [Scenario]
            [Example(1, 2, 3)]
            public static void Scenario(int x, int y, int z) =>
                "Given {3}, {4} and {5}"
                    .x(() => { });
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

        private static class ScenariosExpectingDateTimeValues
        {
            private static readonly DateTime expected = new DateTime(2014, 6, 26, 10, 48, 30);

            [Scenario]
            [Example("2014-06-26")]
            public static void Scenario1(DateTime actual)
            {
                "Then the actual is expected"
                    .x(() => actual.Should().Be(expected.Date));
            }

            [Scenario]
            [Example("Thu, 26 Jun 2014 10:48:30")]
            [Example("2014-06-26T10:48:30.0000000")]
            public static void Scenario2(DateTime actual)
            {
                "Then the actual is expected"
                    .x(() => actual.Should().Be(expected));
            }
        }

        private static class ScenariosExpectingDateTimeOffsetValues
        {
            private static readonly DateTimeOffset expected = new DateTimeOffset(new DateTime(2014, 6, 26, 10, 48, 30));

            [Scenario]
            [Example("2014-06-26")]
            public static void Scenario1(DateTimeOffset actual)
            {
                "Then the actual is expected"
                    .x(() => actual.Should().Be(expected.Date));
            }

            [Scenario]
            [Example("Thu, 26 Jun 2014 10:48:30")]
            [Example("2014-06-26T10:48:30.0000000")]
            public static void Scenario2(DateTimeOffset actual)
            {
                "Then the actual is expected"
                    .x(() => actual.Should().Be(expected));
            }
        }

        private static class ScenariosExpectingGuidValues
        {
            private static readonly Guid expected = new Guid("0b228327-585d-47f9-a5ee-292f96ca085c");

            [Scenario]
            [Example("0b228327-585d-47f9-a5ee-292f96ca085c")]
            [Example("0B228327-585D-47F9-A5EE-292F96CA085C")]
            [Example("0B228327585D47F9A5EE292F96CA085C")]
            public static void Scenario(Guid actual)
            {
                "Then the actual is expected"
                    .x(() => actual.Should().Be(expected));
            }
        }
    }
}
