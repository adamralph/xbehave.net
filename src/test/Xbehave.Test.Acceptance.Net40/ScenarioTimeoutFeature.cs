// <copyright file="ScenarioTimeoutFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
#if NET40 || NET45
    using System.Threading;
#endif
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to prevent very long running tests <-- improve!
    // As a developer
    // I want a feature to fail if a given scenario takes to long to run
    public static class ScenarioTimeoutFeature
    {
#if NET40 || NET45
        private static readonly ManualResetEventSlim @Event = new ManualResetEventSlim();
#endif

        [Scenario]
        public static void ScenarioExecutesFastEnough()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario which does not exceed it's timeout"
                .Given(() => feature = typeof(ScenarioFastEnough));

            "When the test runner runs the feature"
                .When(() => results = feature.RunScenarios().ToArray());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a pass"
                .And(() => results.Should().ContainItemsAssignableTo<Pass>());
        }

#if NET40 || NET45
        [Scenario(Skip = "See https://github.com/xbehave/xbehave.net/issues/93/")]
        public static void ScenarioExecutesTooSlowly()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario which exceeds it's 1ms timeout"
                .Given(() => feature = typeof(ScenarioTooSlow));

            "When the test runner runs the feature"
                .When(() =>
                {
                    @Event.Reset();
                    results = feature.RunScenarios().ToArray();
                })
                .Teardown(() => @Event.Set());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<Fail>());

            "And the result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<Fail>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }

        [Scenario(Skip = "See https://github.com/xbehave/xbehave.net/issues/93/")]
        public static void ScenarioExecutesTooSlowlyInOneStepAndHasASubsequentPassingStep()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario which exceeds it's 1ms timeout"
                .Given(() => feature = typeof(ScenarioTooSlowInOneStepAndHasASubsequentPassingStep));

            "When the test runner runs the feature"
                .When(() =>
                {
                    @Event.Reset();
                    results = feature.RunScenarios().ToArray();
                })
                .Teardown(() => @Event.Set());

            "Then there should be two result"
                .Then(() => results.Count().Should().Be(2));

            "And the results should be failures"
                .And(() => results.Should().ContainItemsAssignableTo<Fail>());

            "And the first result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<Fail>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }
#endif

        private static class ScenarioFastEnough
        {
#pragma warning disable 618
            [Scenario(Timeout = int.MaxValue)]
#pragma warning restore 618
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { });
            }
        }

#if NET40 || NET45
        private static class ScenarioTooSlow
        {
#pragma warning disable 618
            [Scenario(Timeout = 1)]
#pragma warning restore 618
            public static void Scenario()
            {
                "Given something"
                    .Given(() => @Event.Wait());
            }
        }

        private static class ScenarioTooSlowInOneStepAndHasASubsequentPassingStep
        {
#pragma warning disable 618
            [Scenario(Timeout = 1)]
#pragma warning restore 618
            public static void Scenario()
            {
                "Given something"
                    .Given(() => @Event.Wait());

                "Then true is true"
                    .Then(() => true.Should().BeTrue());
            }
        }
#endif
    }
}
