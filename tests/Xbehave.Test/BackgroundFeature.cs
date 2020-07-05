namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    // In order to write less code
    // As a developer
    // I want to add background steps to all the scenarios in a feature
    public class BackgroundFeature : Feature
    {
        [Scenario]
        [Example(typeof(BackgroundWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        [Example(typeof(BackgroundInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        public void BackgroundSteps(Type feature, ITestResultMessage[] results)
        {
            $"Given a {feature}"
                .x(() => { });

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the background steps are run before each scenario"
                .x(() => Assert.All(results, result => Assert.IsAssignableFrom<ITestPassed>(result)));

            "And there are eight results"
                .x(() => Assert.Equal(8, results.Length));

            "And the background steps have '(Background)' in their names"
                .x(() => Assert.All(
                    results.Take(2).Concat(results.Skip(4).Take(2)),
                    result => Assert.Contains("Background", result.Test.DisplayName)));
        }

        private static class BackgroundWithTwoStepsAndTwoScenariosEachWithTwoSteps
        {
            private static int x;

            [Background]
            public static void Background()
            {
                "Given x is incremented"
                    .x(() => ++x);

                "And x is incremented again"
                    .x(() => ++x);
            }

            [Scenario]
            public static void Scenario1()
            {
                "Given x is 2"
                    .x(() => Assert.Equal(2, x));

                "Then I set x to 0"
                    .x(() => x = 0);
            }

            [Scenario]
            public static void Scenario2()
            {
                "Given x is 2"
                    .x(() => Assert.Equal(2, x));

                "Then I set x to 0"
                    .x(() => x = 0);
            }
        }

        private class Base
        {
            protected int X { get; set; }

            [Background]
            public void Background()
            {
                "Given x is incremented"
                    .x(() => ++this.X);

                "And x is incremented again"
                    .x(() => ++this.X);
            }
        }

        private class BackgroundInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps : Base
        {
            [Scenario]
            public void Scenario1()
            {
                "Given x is 2"
                    .x(() => Assert.Equal(2, this.X));

                "Then I set x to 0"
                    .x(() => this.X = 0);
            }

            [Scenario]
            public void Scenario2()
            {
                "Given x is 2"
                    .x(() => Assert.Equal(2, this.X));

                "Then I set x to 0"
                    .x(() => this.X = 0);
            }
        }
    }
}
