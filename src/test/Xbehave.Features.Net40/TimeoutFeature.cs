﻿// <copyright file="TimeoutFeature.cs" company="xBehave.net contributors">
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
    using Xunit.Sdk;

    // In order to prevent very long running tests <-- improve!
    // As a developer
    // I want a feature to fail if a given step takes to long to run
    public static class TimeoutFeature
    {
#if NET40 || NET45
        private static readonly ManualResetEventSlim @Event = new ManualResetEventSlim();
#endif

        [Scenario]
        public static void StepExecutesFastEnough()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with a single step which does not exceed it's timeout"
                .Given(() => feature = typeof(StepFastEnough));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a pass"
                .And(() => results.Should().ContainItemsAssignableTo<PassedResult>());
        }

#if NET40 || NET45
        [Scenario]
        public static void StepExecutesTooSlowly()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with a single step which exceeds it's 1ms timeout"
                .Given(() => feature = typeof(StepTooSlow));

            "When the test runner runs the feature"
                .When(() =>
                {
                    @Event.Reset();
                    results = TestRunner.Run(feature).ToArray();
                })
                .Teardown(() => @Event.Set());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<FailedResult>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }
#endif

        [Scenario]
        public static void ScenarioExecutesFastEnough()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario which does not exceed it's timeout"
                .Given(() => feature = typeof(ScenarioFastEnough));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a pass"
                .And(() => results.Should().ContainItemsAssignableTo<PassedResult>());
        }

#if NET40 || NET45
        [Scenario(Skip = "See https://github.com/xbehave/xbehave.net/issues/93/")]
        public static void ScenarioExecutesTooSlowly()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario which exceeds it's 1ms timeout"
                .Given(() => feature = typeof(ScenarioTooSlow));

            "When the test runner runs the feature"
                .When(() =>
                {
                    @Event.Reset();
                    results = TestRunner.Run(feature).ToArray();
                })
                .Teardown(() => @Event.Set());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<FailedResult>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }

        [Scenario(Skip = "See https://github.com/xbehave/xbehave.net/issues/93/")]
        public static void ScenarioExecutesTooSlowlyInOneStepAndHasASubsequentPassingStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario which exceeds it's 1ms timeout"
                .Given(() => feature = typeof(ScenarioTooSlowInOneStepAndHasASubsequentPassingStep));

            "When the test runner runs the feature"
                .When(() =>
                {
                    @Event.Reset();
                    results = TestRunner.Run(feature).ToArray();
                })
                .Teardown(() => @Event.Set());

            "Then there should be two result"
                .Then(() => results.Count().Should().Be(2));

            "And the results should be failures"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the first result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<FailedResult>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }
#endif
        private static class StepFastEnough
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { })
                    .WithTimeout(int.MaxValue);
            }
        }

#if NET40 || NET45
        private static class StepTooSlow
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => @Event.Wait())
                    .WithTimeout(1);
            }
        }
#endif

        private static class ScenarioFastEnough
        {
            [Scenario(Timeout = int.MaxValue)]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { });
            }
        }

#if NET40 || NET45
        private static class ScenarioTooSlow
        {
            [Scenario(Timeout = 1)]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => @Event.Wait());
            }
        }

        private static class ScenarioTooSlowInOneStepAndHasASubsequentPassingStep
        {
            [Scenario(Timeout = 1)]
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
