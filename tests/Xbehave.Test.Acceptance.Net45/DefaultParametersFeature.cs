// <copyright file="DefaultParametersFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    // In order to have terse code
    // As a developer
    // I want to declare hold local state using scenario method parameters
    public class DefaultParametersFeature : Feature
    {
        [Scenario]
        public void ScenarioWithParameters()
        {
            var feature = default(Type);
            var results = default(ITestResultMessage[]);

            "Given a scenario with four parameters and step asserting each is a default value"
                .x(() => feature = typeof(ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>(
                    results.ToDisplayString("each result should be a pass")));

            "And there should be 4 results"
                .x(() => results.Length.Should().Be(4));

            "And the display name of each result should not contain the parameter values"
                .x(() =>
                {
                    foreach (var result in results)
                    {
                        result.Test.DisplayName.Should().NotContain("w:");
                        result.Test.DisplayName.Should().NotContain("x:");
                        result.Test.DisplayName.Should().NotContain("y:");
                        result.Test.DisplayName.Should().NotContain("z:");
                    }
                });
        }

        private static class ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue
        {
            [Scenario]
            public static void Scenario(string w, int x, object y, int? z)
            {
                "Then w should be the default value of string"
                    .x(() => w.Should().Be(default(string)));

                "And x should be the default value of int"
                    .x(() => x.Should().Be(default(int)));

                "And y should be the default value of object"
                    .x(() => y.Should().Be(default(object)));

                "And z should be the default value of int?"
                    .x(() => z.Should().Be(default(int?)));
            }
        }
    }
}
