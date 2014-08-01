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
        private static object[] arguments;

        [Scenario]
        public static void ScenarioWithParameters()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with a single step and parameters"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndParameters));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then each result should be a success"
                .Then(() => results.Should().ContainItemsAssignableTo<Pass>());

            "Then the scenario should be executed with default values passed as arguments"
                .Then(() => arguments.Should().OnlyContain(obj => (int)obj == default(int)))
                .Teardown(() => arguments = null);
        }

        private static class FeatureWithAScenarioWithASingleStepAndParameters
        {
            [Scenario]
            public static void Scenario(int w, int x, int y, int z)
            {
                "Given {0}, {1}, {2} and {3}"
                    .Given(() => arguments = new object[] { w, x, y, z });
            }
        }
#endif
    }
}
