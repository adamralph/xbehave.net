// <copyright file="StringExtensionsSpecifications.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit
{
    using System;
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Fluent;

    public class StringExtensionsSpecifications
    {
        [Scenario]
        [Example("Given", StepType.Given)]
        [Example("When", StepType.When)]
        [Example("Then", StepType.Then)]
        [Example("But", StepType.But)]
        [Example("And", StepType.And)]
        public void GetStepType(string clause, StepType expected, StepType actual)
        {
            "Given a string starts with '{0}'"
                .Given(() => clause = clause + " foo");
            "When GetStepType() is called"
                .When(() => actual = StringExtensions.GetStepType(clause));
            "Then StepType.{1} is returned"
                .Then(() => actual.Should().Be(expected));
        }

        [Scenario]
        public void InvokeXMethod(IStep step, string clause)
        {
            "Given a string"
                .Given(() => clause = "Given foo");
            "When the do extension method is invoked"
                .When(() => step = clause.f(() => { }));
            "Then a step is returned"
                .Then(() => step.Should().NotBeNull());
        }
    }
}
