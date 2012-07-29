// <copyright file="TypeParametersInDisplayNamesFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xunit.Sdk;

    // In order to distinguish failing scenario examples
    // As a developer
    // I want type parameters to be shown in the display name of each step in the test runner output
    public static class TypeParametersInDisplayNamesFeature
    {
        [Scenario]
        public static void CreatingStepsFromAScenarioWithInt32Examples()
        {
            var scenario = default(IMethodInfo);
            var steps = default(ITestCommand[]);

            "Given a generic scenario with Int32 examples"
                .Given(() => scenario = TestRunner.CreateScenario<int, int, int>(GenericScenarioWithInt32Examples));

            "When the test runner creates steps using the scenario"
                .When(() => steps = TestRunner.CreateSteps(scenario).ToArray());

            "Then the display name of each command should contain <Int32, Int32, Int32>"
                .Then(() => steps.Should().OnlyContain(step => step.DisplayName.Contains(scenario.Name + "<Int32, Int32, Int32>")));
        }

        [Example(1, 2, 3)]
        [Example(3, 4, 5)]
        [Example(5, 6, 7)]
        public static void GenericScenarioWithInt32Examples<T1, T2, T3>(T1 x, T2 y, T3 z)
        {
            "Given"
                .Given(() => { });
            
            "When"
                .When(() => { });

            "Then"
                .Then(() => { });
        }

        [Scenario]
        public static void CreatingStepsFromAScenarioWithInt64Examples()
        {
            var scenario = default(IMethodInfo);
            var steps = default(ITestCommand[]);

            "Given a generic scenario with Int64 examples"
                .Given(() => scenario = TestRunner.CreateScenario<long, long, long>(GenericScenarioWithInt64Examples));

            "When the test runner creates steps using the scenario"
                .When(() => steps = TestRunner.CreateSteps(scenario).ToArray());

            "Then the display name of each command should contain <Int64, Int64, Int64>"
                .Then(() => steps.Should().OnlyContain(step => step.DisplayName.Contains(scenario.Name + "<Int64, Int64, Int64>")));
        }

        [Example(1L, 2L, 3L)]
        [Example(3L, 4L, 5L)]
        [Example(5L, 6L, 7L)]
        public static void GenericScenarioWithInt64Examples<T1, T2, T3>(T1 x, T2 y, T3 z)
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
