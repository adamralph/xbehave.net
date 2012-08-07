// <copyright file="TimeoutFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using System.Threading;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to prevent very long running tests <-- improve!
    // As a developer
    // I want a feature to fail if a given step takes to long to run
    public static class TimeoutFeature
    {
        private static readonly ManualResetEventSlim @Event = new ManualResetEventSlim();

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
    }
}
