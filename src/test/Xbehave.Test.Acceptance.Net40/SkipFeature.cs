// <copyright file="SkipFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System.Collections.Concurrent;
    using System.Linq;
    using FluentAssertions;
    using Xunit.Sdk;

    // In order to commit unfinished features
    // As a developer
    // I want to temporarily skip specific steps
    public static class SkipFeature
    {
        private static readonly ConcurrentBag<object[]> ArgumentLists = new ConcurrentBag<object[]>();

        [Scenario]
        public static void UnfinishedFeature()
        {
            var scenario = default(IMethodInfo);
            var results = default(MethodResult[]);

            "Given a scenario with skipped steps relating to unfinished feature aspects which throw exceptions"
                .Given(() => scenario = TestRunner.CreateScenario(ScenarioWithSkippedStepsRelatingToUnfinishedFeatureAspects));

            "When the test runner executes the scenario"
                .When(() => results = TestRunner.Execute(scenario).ToArray());

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));
        }

        public static void ScenarioWithSkippedStepsRelatingToUnfinishedFeatureAspects()
        {
            "Given something"
                .Given(() => { });

            "When I doing something"
                .When(() => { });

            "Then there is an outcome"
                .Then(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");

            "And there is another outcome"
                .And(() => { throw new System.NotImplementedException(); }).Skip("the feature is unfinished");
        }
    }
}
