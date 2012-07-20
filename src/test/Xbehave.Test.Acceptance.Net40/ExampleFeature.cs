// <copyright file="ExampleFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Sdk.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;
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
            var commands = default(IEnumerable<ITestCommand>);
            var exceptions = default(Exception[]);

            "Given a scenario using examples to assert the equality of 1 and 2 and the equality of 3 and 4"
                .Given(() => method = Reflector.Wrap(typeof(ExampleFeature).GetMethod("AScenarioUsingExamplesToAssertTheEqualityOf1And2AndTheEqualityOf3And4")));

            "When creating test commands from the method"
                .When(() => commands = new ScenarioAttribute().CreateTestCommands(method));

            "And recording the exception thrown when executing each command"
                .And(() => exceptions = Task.Factory.StartNew(() => commands.Select(command => Record.Exception(() => command.Execute(null))).ToArray()).Result);

            "Then the number of exceptions should be 2"
                .Then(() => exceptions.Should().HaveCount(2));

            "And one exception message should contain 1 and 2 and the other exception message should contain 3 and 4"
                .And(() =>
                    ((exceptions[0].Message.Contains("1") &&
                        exceptions[0].Message.Contains("2") &&
                        exceptions[1].Message.Contains("3") &&
                        exceptions[1].Message.Contains("4")) ^
                    (exceptions[1].Message.Contains("1") &&
                        exceptions[1].Message.Contains("2") &&
                        exceptions[0].Message.Contains("3") &&
                        exceptions[0].Message.Contains("4"))).Should().BeTrue());
        }

        [Example(1, 2)]
        [Example(3, 4)]
        public static void AScenarioUsingExamplesToAssertTheEqualityOf1And2AndTheEqualityOf3And4(int x, int y)
        {
            "Then"
                .Then(() => x.Should().Be(y));
        }
    }
}
