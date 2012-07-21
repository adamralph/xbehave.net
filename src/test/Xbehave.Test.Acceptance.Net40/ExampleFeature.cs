// <copyright file="ExampleFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xunit.Extensions;
    using Xunit.Sdk;

    // In order to save time
    // As a developer
    // I want to write a single scenario using many examples
    public static class ExampleFeature
    {
        [Scenario]
        public static void IntegerExamplesWith3Values()
        {
            var method = default(IMethodInfo);
            var commands = default(ITestCommand[]);
            var examples = default(ExampleAttribute[]);
            var theoryCommands = default(TheoryCommand[]);

            "Given a method with a single step using integer examples with 3 values"
                .Given(() => method = Reflector.Wrap(((Action<int, int, int>)ExampleFeature.SingleStepUsingIntegerExamplesWith3Values).Method));

            "When a test runner creates test commands using the method"
                .When(() => commands = new ScenarioAttribute().CreateTestCommands(method).ToArray());

            "Then the number of commands should be match the number of examples"
                .Then(() =>
                {
                    examples = method.GetCustomAttributes(typeof(ExampleAttribute)).Select(x => x.GetInstance<ExampleAttribute>()).ToArray();
                    commands.Should().HaveCount(examples.Length);
                });

            "And the commands should be theory commands"
                .And(() => theoryCommands = commands.Cast<TheoryCommand>().ToArray());

            "And the ordered command arguments and example values should match"
                .And(() =>
                {
                    var args = theoryCommands.Select(command => command.Parameters.Cast<int>().ToArray()).OrderBy(x => x, new ArrayComparer<int>()).ToArray();
                    var values = examples.Select(example => example.DataValues.Cast<int>().ToArray()).OrderBy(x => x, new ArrayComparer<int>()).ToArray();
                    for (var index = 0; index < args.Length; ++index)
                    {
                        args[index].Should().Equal(values[index]);
                    }
                });
        }

        [Example(1, 2, 3)]
        [Example(3, 4, 5)]
        [Example(5, 6, 7)]
        public static void SingleStepUsingIntegerExamplesWith3Values(int x, int y, int z)
        {
            "Given"
                .Given(() => { });
        }
    }
}
