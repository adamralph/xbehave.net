namespace Xbehave.Test
{
    using System;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    // In order to have terse code
    // As a developer
    // I want to declare hold local state using scenario method parameters
    public class DefaultParametersFeature : Feature
    {
        [Scenario]
        public void ScenarioWithParameters(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with four parameters and step asserting each is a default value"
                .x(() => feature = typeof(ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass"
                .x(() => Assert.All(results, result => Assert.IsAssignableFrom<ITestPassed>(result)));

            "And there should be 4 results"
                .x(() => Assert.Equal(4, results.Length));

            "And the display name of each result should not contain the parameter values"
                .x(() => Assert.All(
                    results,
                    result =>
                    {
                        Assert.DoesNotContain("w:", result.Test.DisplayName);
                        Assert.DoesNotContain("x:", result.Test.DisplayName);
                        Assert.DoesNotContain("y:", result.Test.DisplayName);
                        Assert.DoesNotContain("z:", result.Test.DisplayName);
                    }));
        }

        private static class ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue
        {
            [Scenario]
            public static void Scenario(string w, int x, object y, int? z)
            {
                "Then w should be the default value of string"
                    .x(() => Assert.Equal(default, w));

                "And x should be the default value of int"
                    .x(() => Assert.Equal(default, x));

                "And y should be the default value of object"
                    .x(() => Assert.Equal(default, y));

                "And z should be the default value of int?"
                    .x(() => Assert.Equal(default, z));
            }
        }
    }
}
