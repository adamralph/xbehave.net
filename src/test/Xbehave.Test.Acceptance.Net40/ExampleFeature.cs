// <copyright file="ExampleFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System.Collections.Concurrent;
    using System.Linq;
    using FluentAssertions;
    using Xunit.Sdk;

    // In order to save time
    // As a developer
    // I want to write a single scenario using many examples
    public static class ExampleFeature
    {
        private static readonly ConcurrentBag<object[]> ArgumentLists = new ConcurrentBag<object[]>();

        [Scenario]
        public static void ExecutionOfScenariosWithExamples()
        {
            var scenario = default(IMethodInfo);

            "Given a scenario with examples"
                .Given(() => scenario = TestRunner.CreateScenario<object, object, object>(ScenarioWithASingleStepAndExamples));

            "When the test runner executes the scenario"
                .When(() => TestRunner.Execute(scenario));

            "Then the scenario should be executed once for each example with the values from that example passed as arguments"
                .Then(() =>
                {
                    ArgumentLists.Select(arguments => arguments.Cast<object>()).OrderBy(x => x, new EnumerableComparer<object>())
                        .SequenceEqual(
                            scenario.GetCustomAttributes(typeof(ExampleAttribute)).Select(x => x.GetInstance<ExampleAttribute>())
                                .Select(example => example.DataValues.Cast<object>()).OrderBy(x => x, new EnumerableComparer<object>()),
                            new EnumerableEqualityComparer<object>()).Should().BeTrue();
                });
        }

        [Example(1, 2L, "a")]
        [Example(null, 2U, "a")]
        public static void ScenarioWithASingleStepAndExamples<T1, T2, T3>(T1 x, T2 y, T3 z)
        {
            "Given {0}, {1} and {2}"
                .Given(() => ArgumentLists.Add(new object[] { x, y, z }));
        }
    }
}
