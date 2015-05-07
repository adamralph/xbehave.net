// <copyright file="MetadataFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using FluentAssertions;
    using Xbehave.Sdk;
    using Xunit.Sdk;

    public class MetadataFeature
    {
        [Scenario]
        [Example("abc")]
        public void UsingMetadata(
            string text, IStepContext stepContext, IStep step, IScenario scenario, IXunitTestCase scenarioOutline)
        {
            "When I execute a step"
                .f(c => stepContext = c);

            "Then the step context contains metadata about the step"
                .f(() => (step = stepContext.Step.Should().NotBeNull().And.Subject.As<IStep>())
                    .DisplayName.Should().Be("Xbehave.Test.Acceptance.MetadataFeature.UsingMetadata(text: \"abc\") [01] When I execute a step"));

            "And the step contains metadata about the scenario"
                .f(() => (scenario = step.Scenario.Should().NotBeNull().And.Subject.As<IScenario>())
                    .DisplayName.Should().Be("Xbehave.Test.Acceptance.MetadataFeature.UsingMetadata(text: \"abc\")"));

            "And the step contains metadata about the scenario outline"
                .f(() => scenario.ScenarioOutline.Should().NotBeNull().And.Subject.As<IXunitTestCase>()
                    .DisplayName.Should().Be("Xbehave.Test.Acceptance.MetadataFeature.UsingMetadata"));
        }
    }
}
#endif
