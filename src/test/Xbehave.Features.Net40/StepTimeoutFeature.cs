// <copyright file="StepTimeoutFeature.cs" company="xBehave.net contributors">
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
    using Xbehave.Features.Infrastructure;
    using Xbehave.Test.Acceptance.Infrastructure;

    // In order to prevent very long running tests <-- improve!
    // As a developer
    // I want a feature to fail if a given step takes to long to run
    public static class StepTimeoutFeature
    {
#if NET40 || NET45
        private static readonly ManualResetEventSlim @Event = new ManualResetEventSlim();
#endif

        [Scenario]
        public static void StepExecutesFastEnough()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a feature with a scenario with a single step which does not exceed it's timeout"
                .Given(() => feature = typeof(StepFastEnough));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a pass"
                .And(() => results.Should().ContainItemsAssignableTo<Pass>());
        }

#if NET40 || NET45
        [Scenario]
        public static void StepExecutesTooSlowly()
        {
            var feature = default(Type);
            var results = default(Result[]);

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
                .And(() => results.Should().ContainItemsAssignableTo<Fail>());

            "And the result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<Fail>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
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
    }
}
