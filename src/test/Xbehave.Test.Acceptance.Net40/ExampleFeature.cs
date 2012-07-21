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
        public static void DifferingIntegerExamples()
        {
            var method = default(IMethodInfo);
            var commands = default(ITestCommand[]);
            var theoryCommands = default(TheoryCommand[]);

            "Given a scenario with a single step using example values 1 and 2 and example values 3 and 4"
                .Given(() => method = Reflector.Wrap(((Action<int, int>)ExampleFeature.AScenarioWithASingleStepUsingExampleValues1And2AndExampleValues3And4).Method));

            "When a test runner creates test commands from the method"
                .When(() => commands = new ScenarioAttribute().CreateTestCommands(method).ToArray());

            "Then the number of commands should be 2"
                .Then(() => commands.Should().HaveCount(2));

            "And the commands should be theory commands"
                .And(() =>
                {
                    commands.Should().ContainItemsAssignableTo<TheoryCommand>();
                    theoryCommands = commands.Cast<TheoryCommand>().ToArray();
                });

            "And each command should have 2 integer arguments"
                .And(() => theoryCommands.Should().OnlyContain(command => command.Parameters.Count() == 2 && command.Parameters.All(parameter => parameter is int)));

            "And one command should have arguments 1 and 2 and the other command should have arguments 3 and 4"
                .And(() =>
                {
                    var orderedCommands = theoryCommands.OrderBy(command => (int)command.Parameters.ElementAt(0)).ToArray();
                    orderedCommands[0].Parameters.ElementAt(0).Should().Be(1);
                    orderedCommands[0].Parameters.ElementAt(1).Should().Be(2);
                    orderedCommands[1].Parameters.ElementAt(0).Should().Be(3);
                    orderedCommands[1].Parameters.ElementAt(1).Should().Be(4);
                });
        }

        [Example(1, 2)]
        [Example(3, 4)]
        public static void AScenarioWithASingleStepUsingExampleValues1And2AndExampleValues3And4(int x, int y)
        {
            "Given"
                .Given(() => { });
        }
    }
}
