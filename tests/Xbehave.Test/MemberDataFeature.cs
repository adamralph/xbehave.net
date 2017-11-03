namespace Xbehave.Test
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class MemberDataFeature : Feature
    {
        [Scenario]
        [Example(typeof(AScenarioUsingMemberDataProperty))]
        [Example(typeof(AScenarioUsingMemberDataMethod))]
        [Example(typeof(AScenarioUsingMemberDataField))]
        [Example(typeof(AScenarioUsingNonSerializableValues))]
        public void MemberDataProperty(Type feature, ITestResultMessage[] results)
        {
            $"Given {feature}"
                .x(() => { });

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .x(() => results.Length.Should().Be(3));

            "Then each of the member data value sets should be passed into the scenario"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

        public class AScenarioUsingMemberDataProperty
        {
            private static int previousSum;

            public static IEnumerable<object[]> MemberData
            {
                get
                {
                    yield return new object[] { 1, 2, 3 };
                    yield return new object[] { 10, 20, 30 };
                    yield return new object[] { 100, 200, 300 };
                }
            }

            [Scenario]
            [MemberData(nameof(MemberData))]
            public void Scenario(int operand1, int operand2, int sum) =>
                $"Then as a distinct example the sum of {operand1} and {operand2} is {sum}"
                    .x(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (operand1 + operand2).Should().Be(sum);
                        previousSum = sum;
                    });
        }

        public class AScenarioUsingMemberDataMethod
        {
            private static int previousSum;

            public static IEnumerable<object[]> MemberData()
            {
                yield return new object[] { 1, 2, 3 };
                yield return new object[] { 10, 20, 30 };
                yield return new object[] { 100, 200, 300 };
            }

            [Scenario]
            [MemberData(nameof(MemberData))]
            public void Scenario(int operand1, int operand2, int sum) =>
                $"Then as a distinct example the sum of {operand1} and {operand2} is {sum}"
                    .x(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (operand1 + operand2).Should().Be(sum);
                        previousSum = sum;
                    });
        }

        public class AScenarioUsingMemberDataField
        {
            public static readonly IEnumerable<object[]> MemberData = new List<object[]>
            {
                new object[] { 1, 2, 3 },
                new object[] { 10, 20, 30 },
                new object[] { 100, 200, 300 },
            };

            private static int previousSum;

            [Scenario]
            [MemberData(nameof(MemberData))]
            public void Scenario(int operand1, int operand2, int sum) =>
                $"Then as a distinct example the sum of {operand1} and {operand2} is {sum}"
                    .x(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (operand1 + operand2).Should().Be(sum);
                        previousSum = sum;
                    });
        }

        public class AScenarioUsingNonSerializableValues
        {
            public static readonly IEnumerable<object[]> MemberData = new List<object[]>
            {
                new[] { new DoesNotSerialize { Value = 1 } },
                new[] { new DoesNotSerialize { Value = 2 } },
                new[] { new DoesNotSerialize { Value = 3 } },
            };

            private static int previousValue;

            [Scenario]
            [MemberData(nameof(MemberData))]
            public void Scenario(DoesNotSerialize @object) =>
                $"Then the object has a distinct value of {@object.Value}"
                    .x(() =>
                    {
                        @object.Value.Should().NotBe(previousValue);
                        previousValue = @object.Value;
                    });

            public class DoesNotSerialize
            {
                public int Value { get; set; }
            }
        }
    }
}
