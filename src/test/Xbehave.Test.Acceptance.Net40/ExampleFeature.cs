// <copyright file="ExampleFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
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
            var feature = default(Type);

            "Given a feature with a scenario with examples"
                .Given(() => feature = typeof(FeatureWithAScenarioWithASingleStepAndExamples));

            "When the test runner runs the feature"
                .When(() => TestRunner.Run(feature));

            "Then the scenario should be executed once for each example with the values from that example passed as arguments"
                .Then(() =>
                {
                    ArgumentLists.Select(arguments => arguments.Cast<int>()).OrderBy(x => x, new EnumerableComparer<int>())
                        .SequenceEqual(
                            Reflector.Wrap(feature.GetMethod("Scenario")).GetCustomAttributes(typeof(ExampleAttribute)).Select(x => x.GetInstance<ExampleAttribute>())
                                .Select(example => example.DataValues.Cast<int>()).OrderBy(x => x, new EnumerableComparer<int>()),
                            new EnumerableEqualityComparer<int>()).Should().BeTrue();
                });
        }

        private static class FeatureWithAScenarioWithASingleStepAndExamples
        {
            [Scenario]
            [Example(1, 2, 3)]
            [Example(3, 4, 5)]
            [Example(5, 6, 7)]
            public static void Scenario(int x, int y, int z)
            {
                "Given {0}, {1} and {2}"
                    .Given(() => ArgumentLists.Add(new object[] { x, y, z }));
            }
        }
    }
}
