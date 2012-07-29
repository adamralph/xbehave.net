// <copyright file="ExampleFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
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
        public static void Int32Examples()
       { 
            var scenario = default(IMethodInfo);

            "Given a scenario with a single step and Int32 examples"
                .Given(() => scenario = Reflector.Wrap(((Action<int, int, int>)SingleStepUsingInt32Examples).Method));

            "When the test runner executes the scenario"
                .When(() => TestRunner.Execute(scenario));

            "Then the ordered argument lists and example value lists should match"
                .Then(() =>
                {
                    ArgumentLists.Select(arguments => arguments.Cast<int>()).OrderBy(x => x, new EnumerableComparer<int>())
                        .SequenceEqual(
                            scenario.GetCustomAttributes(typeof(ExampleAttribute)).Select(x => x.GetInstance<ExampleAttribute>())
                                .Select(example => example.DataValues.Cast<int>()).OrderBy(x => x, new EnumerableComparer<int>()),
                            new EnumerableEqualityComparer<int>()).Should().BeTrue();
                });
        }

        [Example(1, 2, 3)]
        [Example(3, 4, 5)]
        [Example(5, 6, 7)]
        public static void SingleStepUsingInt32Examples(int x, int y, int z)
        {
            "Given {0}, {1} and {2}"
                .Given(() => ArgumentLists.Add(new object[] { x, y, z }));
        }
    }
}
