// <copyright file="DefaultParametersFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Features.Infrastructure;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to have terse code
    // As a developer
    // I want to declare hold local state using scenario method parameters
    public static class DefaultParametersFeature
    {
#if !V2
        [Scenario]
        public static void ScenarioWithParameters()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with four parameters and step asserting each one is a default value"
                .f(() => feature = typeof(FeatureWithAScenarioWithAFourParametersAndAStepAssertingEachOneIsADefaultValue));

            "When the test runner runs the feature"
                .f(() => results = TestRunner.Run(feature).ToArray());

            "Then each result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>(
                    results.ToDisplayString("each result should be a pass")));

            "And there should be 4 results"
                .f(() => results.Length.Should().Be(4));
        }

        private static class FeatureWithAScenarioWithAFourParametersAndAStepAssertingEachOneIsADefaultValue
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
#endif
    }
}
