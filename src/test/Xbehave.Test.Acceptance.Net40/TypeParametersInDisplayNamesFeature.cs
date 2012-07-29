// <copyright file="TypeParametersInDisplayNamesFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to check which types of values are being used in scenario examples
    // As a product owner
    // I want the display name of each step to contain the types of the values used
    public static class TypeParametersInDisplayNamesFeature
    {
        [Scenario]
        public static void CreatingStepsFromAGenericScenario()
        {
            var scenario = default(IMethodInfo);
            var steps = default(ITestCommand[]);

            "Given a generic scenario with examples containing an Int32 value, an Int64 value and a String value"
                .Given(() => scenario = TestRunner.CreateScenario<int, long, string>(GenericScenario));

            "When the test runner creates steps from the scenario"
                .When(() => steps = TestRunner.CreateSteps(scenario).ToArray());

            "Then the display name of each step should contain \"<Int32, Int64, String>\""
                .Then(() => steps.Should().OnlyContain(step => step.DisplayName.Contains("<Int32, Int64, String>")));
        }

        [Example(1, 2L, "a")]
        [Example(3, 4L, "a")]
        [Example(5, 6L, "a")]
        public static void GenericScenario<T1, T2, T3>(T1 x, T2 y, T3 z)
        {
            "Given"
                .Given(() => { });

            "When"
                .When(() => { });

            "Then"
                .Then(() => { });
        }
    }
}
