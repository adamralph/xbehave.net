// <copyright file="InIsolationFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to define steps which would cause following steps to fail
    // As a developer
    // I want to execute steps in isolation from following steps
    public class InIsolationFeature
    {
        [Scenario]
        public static void IsolatedSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with isolated steps which would cause following steps to fail if executed in the same context"
                .Given(() => feature = typeof(FeatureWithAScenarioWithIsolatedStepsWhichWouldCauseFollowingStepsToFailIfExecutedInTheSameContext));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And there should be no failures"
                .And(() => results.Should().NotContain(result => result is FailedResult));
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
    }
}
