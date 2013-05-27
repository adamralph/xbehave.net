// <copyright file="StringExtensionsSpecifications.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Specifications
{
    using System;
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Fluent;

    public class StringExtensionsSpecifications
    {
        [Scenario]
        public void GetStepType()
        {
            StepType stepType = StepType.Any;
            string clause = null;

            "Given a string starts with 'GIVEN'" 
                .Given(() => clause = "Given foo");
            "When GetStepType() is called"
                .When(() => stepType = StringExtensions.GetStepType(clause));
            "Then the StepType.Give is returned"
                .Then(() => stepType.Should().Be(StepType.Given));

            "Given a string starts with 'WHEN'"
                .Given(() => clause = "When foo");
            "When GetStepType() is called"
                .When(() => stepType = StringExtensions.GetStepType(clause));
            "Then the StepType.When is returned"
                .Then(() => stepType.Should().Be(StepType.When));

            "Given a string starts with 'THEN'"
                .Given(() => clause = "Then foo");
            "When GetStepType() is called"
                .When(() => stepType = StringExtensions.GetStepType(clause));
            "Then the StepType.Then is returned"
                .Then(() => stepType.Should().Be(StepType.Then));

            "Given a string starts with 'AND'"
                .Given(() => clause = "And foo");
            "When GetStepType() is called"
                .When(() => stepType = StringExtensions.GetStepType(clause));
            "Then the StepType.And is returned"
                .Then(() => stepType.Should().Be(StepType.And));

            "Given a string starts with 'BUT'"
                .Given(() => clause = "But foo");
            "When GetStepType() is called"
                .When(() => stepType = StringExtensions.GetStepType(clause));
            "Then the StepType.But is returned"
                .Then(() => stepType.Should().Be(StepType.But));
        }
        
        [Scenario]
        public void InvokeXMethod()
        {
            IStep step = null;
            string clause = null;

            "Given a string"
                .Given(() => clause = "Given foo");
            "When the X extension method is invoked"
                .When(() => step = clause.X(
                    () =>
                        {
                        }));
            "Then a step is returned"
                .Then(() => step.Should().NotBeNull());
        }
    }
} 
