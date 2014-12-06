// <copyright file="DefaultParametersFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to have terse code
    // As a developer
    // I want to declare hold local state using scenario method parameters
    public static class DefaultParametersFeature
    {
        [Scenario]
        public static void ScenarioWithParameters()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a scenario with four parameters and step asserting each is a default value"
                .f(() => feature = typeof(ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "Then each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>(
                    results.ToDisplayString("each result should be a pass")));

            "And there should be 4 results"
                .f(() => results.Length.Should().Be(4));

            "And the display name of each result should not contain the parameter values"
                .f(() =>
                {
                    foreach (var result in results)
                    {
                        result.DisplayName.Should().NotContain("w:");
                        result.DisplayName.Should().NotContain("x:");
                        result.DisplayName.Should().NotContain("y:");
                        result.DisplayName.Should().NotContain("z:");
                    }
                });
        }

        private static class ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue
        {
            [Scenario]
            public static void Scenario(string w, int x, object y, int? z)
            {
                "Then w should be the default value of string"
                    .f(() => w.Should().Be(default(string)));

                "And x should be the default value of int"
                    .f(() => x.Should().Be(default(int)));

                "And y should be the default value of object"
                    .f(() => y.Should().Be(default(object)));

                "And z should be the default value of int?"
                    .f(() => z.Should().Be(default(int?)));
            }
        }
    }
}
