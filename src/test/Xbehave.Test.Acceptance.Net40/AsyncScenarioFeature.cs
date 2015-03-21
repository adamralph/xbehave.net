// <copyright file="AsyncScenarioFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if NET45
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    public class AsyncScenarioFeature : Feature
    {
        [Scenario]
        public void AsyncScenario(Type feature, ITestResultMessage[] results)
        {
            "Given an async scenario"
                .f(() => feature = typeof(FeatureWithAsyncScenario));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then the result should be a pass"
                .f(() => results.Should().ContainItemsAssignableTo<ITestPassed>());
        }

        private static class FeatureWithAsyncScenario
        {
            [Scenario]
            public static async Task Scenario()
            {
                "Given"
                    .f(() => { });

                await Task.FromResult(0);
            }
        }
    }
}
#endif
