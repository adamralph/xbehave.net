// <copyright file="InIsolationFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to define steps which would cause following steps to fail
    // As a developer
    // I want to execute steps in isolation from following steps
    public static class InIsolationFeature
    {
        [Scenario]
        public static void UnfinishedFeature()
        {
            var scenario = default(IMethodInfo);
            var results = default(MethodResult[]);

            "Given a scenario with isolated steps which would cause following steps to fail if executed in the same context"
                .Given(() => scenario = TestRunner.CreateScenario(ScenarioWithIsolatedStepsWhichWouldCauseFollowingStepsToFailIfExecutedInTheSameContext2));

            "When the test runner executes the scenario"
                .When(() => results = TestRunner.Execute(scenario).ToArray());

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));
        }

        public static void ScenarioWithIsolatedStepsWhichWouldCauseFollowingStepsToFailIfExecutedInTheSameContext2()
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
                .And(() => stack.Any());

            "But the stack should not contain more than one item"
                .But(() => stack.Count.Should().BeLessOrEqualTo(1));
        }
    }
}
