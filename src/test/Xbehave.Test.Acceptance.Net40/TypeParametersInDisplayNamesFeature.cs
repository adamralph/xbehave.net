// <copyright file="TypeParametersInDisplayNamesFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xunit.Extensions;
    using Xunit.Sdk;

    // In order to distinguish failing scenario examples
    // As a developer
    // I want type parameters to be shown in the display name of each step in the test runner output
    public static class TypeParametersInDisplayNamesFeature
    {
        [Scenario]
        public static void Int32Examples()
        {
            var method = default(IMethodInfo);
            var commands = default(ITestCommand[]);

            "Given a generic method with a single step using Int32 examples"
                .Given(() => method = Reflector.Wrap(((Action<int, int, int>)SingleStepUsingInt32Examples<int, int, int>).Method));

            "When a test runner creates test commands using the method"
                .When(() => commands = new ScenarioAttribute().CreateTestCommands(method).ToArray());

            "Then the display name of each command should contain <Int32, Int32, Int32>"
                .Then(() => commands.Should().OnlyContain(command => command.DisplayName.Contains(method.Name + "<Int32, Int32, Int32>")));
        }

        [Example(1, 2, 3)]
        [Example(3, 4, 5)]
        [Example(5, 6, 7)]
        public static void SingleStepUsingInt32Examples<T1, T2, T3>(T1 x, T2 y, T3 z)
        {
            "Given"
                .Given(() => { });
        }

        [Scenario]
        public static void LongExamples()
        {
            var method = default(IMethodInfo);
            var commands = default(ITestCommand[]);

            "Given a generic method with a single step using Int64 examples"
                .Given(() => method = Reflector.Wrap(((Action<long, long, long>)SingleStepUsingInt64Examples<long, long, long>).Method));

            "When a test runner creates test commands using the method"
                .When(() => commands = new ScenarioAttribute().CreateTestCommands(method).ToArray());

            "Then the display name of each command should contain <Int64, Int64, Int64>"
                .Then(() => commands.Should().OnlyContain(command => command.DisplayName.Contains(method.Name + "<Int64, Int64, Int64>")));
        }

        [Example(1L, 2L, 3L)]
        [Example(3L, 4L, 5L)]
        [Example(5L, 6L, 7L)]
        public static void SingleStepUsingInt64Examples<T1, T2, T3>(T1 x, T2 y, T3 z)
        {
            "Given"
                .Given(() => { });
        }
    }
}
