// <copyright file="TypeParametersInDisplayNamesFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
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
        public static void RunningAGenericScenario()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a generic scenario with examples containing an Int32 value, an Int64 value and a String value"
                .Given(() => feature = typeof(FeatureWithGenericScenario));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the display name of each result should contain \"<Int32, Int64, String>\""
                .And(() => results.Should().OnlyContain(step => step.DisplayName.Contains("<Int32, Int64, String>")));
        }

        private static class FeatureWithGenericScenario
        {
            [Scenario]
            [Example(1, 2L, "a")]
            [Example(3, 4L, "a")]
            [Example(5, 6L, "a")]
            public static void Scenario<T1, T2, T3>(T1 x, T2 y, T3 z)
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
}
