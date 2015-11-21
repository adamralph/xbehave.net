// <copyright file="ArgumentFormattingFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>
namespace Xbehave.Test.Acceptance
{
    using FluentAssertions;
    using Xbehave.Sdk;
    using Xbehave.Test.Acceptance.Infrastructure;

    public class ArgumentFormattingFeature : Feature
    {
        [Scenario]
        [Example("hello world", 42)]
        public void BaseTypes(string s, int i, IStepContext stepContext)
        {
            "When I execute a step"
                .f(c => stepContext = c);

            "Then the step's display name should contain the example data"
                .f(() => stepContext.Step.DisplayName
                    .Should().Match("*(s: \"hello world\", i: 42)*"));
        }

        [Scenario]
        [Example(new[] { "one", "two" }, new[] { 1, 2 })]
        public void Arrays(string[] words, int[] numbers, IStepContext stepContext)
        {
            "When I execute a step"
                .f(c => stepContext = c);

            "Then the step's display name should contain the example data"
                .f(() => stepContext.Step.DisplayName
                    .Should().Match("*(words: [\"one\", \"two\"], numbers: [1, 2])*"));
        }
    }
}
