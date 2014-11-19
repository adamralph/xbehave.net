// <copyright file="IsolatedStepFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if !V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to define steps which would cause following steps to fail
    // As a developer
    // I want to execute steps in isolation from following steps
    public static class IsolatedStepFeature
    {
        [Scenario]
        public static void IsolatedSteps()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with isolated steps which would cause following steps to fail if executed in the same context"
                .Given(() => feature = typeof(FeatureWithAScenarioWithIsolatedStepsWhichWouldCauseFollowingStepsToFailIfExecutedInTheSameContext));

            "When I run the scenarios"
                .When(() => results = feature.RunScenarios());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .And(() => results.Should().NotContain(result => result is Fail));
        }

        [Scenario]
        public static void OrderingStepsByDisplayName()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a step 'z' and an isolated step 'y'"
                .f(() => feature = typeof(AStepZAnIsolatedStepYAndAStepX));

            "When I run the scenarios"
                .f(() => results = feature.RunScenarios());

            "And I sort the results by their display name"
                .f(() => results = results.OrderBy(result => result.DisplayName).ToArray());

            "Then the first result should have a display name ending with 'z'"
                .f(() => results[0].DisplayName.Should().EndWith("z"));

            "And the second result should have a display name ending with 'y'"
                .f(() => results[1].DisplayName.Should().EndWith("y"));

            "And the third result should have a display name ending with 'z'"
                .f(() => results[2].DisplayName.Should().EndWith("z"));

            "And the fourth result should have a display name ending with 'x'"
                .f(() => results[3].DisplayName.Should().EndWith("x"));
        }

        private static class FeatureWithAScenarioWithIsolatedStepsWhichWouldCauseFollowingStepsToFailIfExecutedInTheSameContext
        {
            [Scenario]
            public static void Scenario()
            {
                var stack = default(System.Collections.Generic.Stack<int>);

                "Given a stack"
                    .Given(() => stack = new System.Collections.Generic.Stack<int>());

                "When pushing 1 onto the stack"
                    .When(() => stack.Push(1));

                "Then the first item in the stack should be 1"
                    .Then(() => stack.Pop().Should().Be(1))
                    .InIsolation();

                "And the first item in the stack should be 1"
                    .And(() => stack.Pop().Should().Be(1))
                    .InIsolation();

                "And the stack should contain some items"
                    .And(() => stack.Should().NotBeEmpty());

                "But the stack should not contain more than one item"
                    .But(() => stack.Count.Should().BeLessOrEqualTo(1));
            }
        }

        private static class AStepZAnIsolatedStepYAndAStepX
        {
            [Scenario]
            public static void Scenario()
            {
                "z"
                    .f(() => { });

                "y"
                    .f(() => { }).InIsolation();

                "x"
                    .f(() => { });
            }
        }
    }
}
#endif