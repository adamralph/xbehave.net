// <copyright file="MemberDataFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class MemberDataFeature : Feature
    {
        [Scenario]
        [Example(typeof(AScenarioUsingMemberDataProperty))]
        [Example(typeof(AScenarioUsingMemberDataMethod))]
        [Example(typeof(AScenarioUsingMemberDataField))]
        public void MemberDataProperty(Type feature, ITestResultMessage[] results)
        {
            "Given {0}"
                .f(() => { });

            "When I run the scenario"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .f(() => results.Length.Should().Be(3));

            "Then each of the member data value sets should be passed into the scenario"
                .f(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
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
            [MemberData("MemberData")]
            public void Scenario(int operand1, int operand2, int sum)
            {
                "Then as a distinct example the sum of {0} and {1} is {2}"
                    .f(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (operand1 + operand2).Should().Be(sum);
                        previousSum = sum;
                    });
            }
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
            [MemberData("MemberData")]
            public void Scenario(int operand1, int operand2, int sum)
            {
                "Then as a distinct example the sum of {0} and {1} is {2}"
                    .f(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (operand1 + operand2).Should().Be(sum);
                        previousSum = sum;
                    });
            }
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
            [MemberData("MemberData")]
            public void Scenario(int operand1, int operand2, int sum)
            {
                "Then as a distinct example the sum of {0} and {1} is {2}"
                    .f(() =>
                    {
                        sum.Should().NotBe(previousSum);
                        (operand1 + operand2).Should().Be(sum);
                        previousSum = sum;
                    });
            }
        }
    }
}
#endif
