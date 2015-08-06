// <copyright file="StringExtensionsSpecifications.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Component
{
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Fluent;

    public static class StringExtensionsSpecifications
    {
        [Scenario]
        [Example("Given", StepType.Given)]
        [Example("When", StepType.When)]
        [Example("Then", StepType.Then)]
        [Example("But", StepType.But)]
        [Example("And", StepType.And)]
        public static void GetStepType(string clause, StepType expected, StepType actual)
        {
            "Given a string starts with '{0}'"
                .Given(() => clause = clause + " foo");

            "When getting the step type"
                .When(() => actual = StringExtensions.GetStepType(clause));

            "Then the step type is '{1}'"
                .Then(() => actual.Should().Be(expected));
        }

        [Scenario]
        public static void InvokedMethod(IStep step, string clause)
        {
            "Given a string"
                .Given(() => clause = "Given foo");

            "When the do extension method is invoked"
                .When(() => step = clause.f(() => { }));

            "Then a step is returned"
                .Then(() => step.Should().NotBeNull());
        }

        [Scenario]
        public static void InvokeUnderscoreMethod(IStep step, string clause)
        {
            "Given a string"
                .Given(() => clause = "Given foo");

            "When the do extension method is invoked"
                .When(() => step = clause._(() => { }));

            "Then a step is returned"
                .Then(() => step.Should().NotBeNull());
        }
    }
}
