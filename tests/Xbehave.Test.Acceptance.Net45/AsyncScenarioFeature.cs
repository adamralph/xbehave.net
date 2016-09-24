// <copyright file="AsyncScenarioFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave.Sdk;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    public class AsyncScenarioFeature : Feature
    {
        [Scenario]
        public void AsyncScenario(Type feature, ITestResultMessage[] results)
        {
            "Given an async scenario"
                .x(() => feature = typeof(FeatureWithAsyncScenario));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the result should be a pass"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

        [Scenario]
        public void NullStepBody()
        {
            "Given a null body"
                .x(default(Func<Task>));
        }

        [Scenario]
        public void NullContextualStepBody()
        {
            "Given a null body"
                .x(default(Func<IStepContext, Task>));
        }

        private static class FeatureWithAsyncScenario
        {
            [Scenario]
            public static async Task Scenario()
            {
                "Given"
                    .x(() => { });

                await Task.FromResult(0);
            }
        }
    }
}
