namespace Xbehave.Test
{
    using FluentAssertions;
    using Xbehave.Sdk;
    using Xunit.Sdk;

    public class MetadataFeature
    {
        [Scenario]
        [Example("abc")]
        public void UsingMetadata(string text, IStepContext stepContext, IStep step, IScenario scenario)
        {
            "When I execute a step"
                .x(c => stepContext = c)
                .Teardown(c => c.Should().BeSameAs(stepContext));

            "Then the step context contains metadata about the step"
                .x(() => (step = stepContext.Step.Should().NotBeNull().And.Subject.As<IStep>())
                    .DisplayName.Should().Be("Xbehave.Test.MetadataFeature.UsingMetadata(text: \"abc\") [01] When I execute a step"));

            "And the step contains metadata about the scenario"
                .x(() => (scenario = step.Scenario.Should().NotBeNull().And.Subject.As<IScenario>())
                    .DisplayName.Should().Be("Xbehave.Test.MetadataFeature.UsingMetadata(text: \"abc\")"));

            "And the step contains metadata about the scenario outline"
                .x(() => scenario.ScenarioOutline.Should().NotBeNull().And.Subject.As<IXunitTestCase>()
                    .DisplayName.Should().Be("Xbehave.Test.MetadataFeature.UsingMetadata"));
        }
    }
}
