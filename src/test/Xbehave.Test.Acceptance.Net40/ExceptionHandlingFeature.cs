// <copyright file="ExceptionHandlingFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;
    using Xunit.Sdk;

    // In order to prevent infinite loops in test runners
    // As a developer
    // I want an exception thrown during step definition to be handled as a test failure
    public static class ExceptionHandlingFeature
    {
        [Scenario]
        public static void ExecutingAScenarioWithInvalidExamples()
        {
            var scenario = default(IMethodInfo);
            var exception = default(Exception);
            var results = default(MethodResult[]);

            "Given a scenario which throws an exception during step definition"
                .Given(() => scenario = TestRunner.CreateScenario<int>(ScenarioWithInvalidExamples));

            "When the test runner executes the scenario"
                .When(() => exception = Record.Exception(() => results = TestRunner.Execute(scenario).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should be failures"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());
        }

        [Example("a")]
        public static void ScenarioWithInvalidExamples(int i)
        {
        }

        [Scenario]
        public static void ExecutingAScenarioWhichThrowsAnExceptionDuringStepDefinition()
        {
            var scenario = default(IMethodInfo);
            var exception = default(Exception);
            var results = default(MethodResult[]);

            "Given a scenario which throws an exception during step definition"
                .Given(() => scenario = TestRunner.CreateScenario(ScenarioWhichThrowsAnExceptionDuringStepDefinition));

            "When the test runner executes the scenario"
                .When(() => exception = Record.Exception(() => results = TestRunner.Execute(scenario).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should be failures"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());
        }

        public static void ScenarioWhichThrowsAnExceptionDuringStepDefinition()
        {
            throw new InvalidOperationException();
        }
    }
}
