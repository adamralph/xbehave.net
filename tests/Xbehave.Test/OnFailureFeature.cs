namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class OnFailureFeature : Feature
    {
        [Scenario]
        public void FailureBeforeForcedStep(Type feature, ITestResultMessage[] results)
        {
            ("Given a scenario with two empty steps, " +
                "a failing step with continuation, " +
                "two more empty steps, " +
                "a failing step and " +
                "another empty step")
                .x(() => feature = typeof(Steps));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be 7 results"
                .x(() => Assert.Equal(7, results.Length));

            "Then the first and second results are passes"
                .x(() => Assert.All(results.Take(2), result => Assert.IsAssignableFrom<ITestPassed>(result)));

            "And the third result is a failure"
                .x(() => Assert.All(results.Skip(2).Take(1), result => Assert.IsAssignableFrom<ITestFailed>(result)));

            "And the fourth and fifth results are passes"
                .x(() => Assert.All(results.Skip(3).Take(2), result => Assert.IsAssignableFrom<ITestPassed>(result)));

            "And the sixth result is a failure"
                .x(() => Assert.All(results.Skip(5).Take(1), result => Assert.IsAssignableFrom<ITestFailed>(result)));

            "And the seventh result is a skip"
                .x(() => Assert.All(results.Skip(6).Take(1), result => Assert.IsAssignableFrom<ITestSkipped>(result)));
        }

        private static class Steps
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .x(() => { });

                "When something"
                    .x(() => { });

                "Then something which fails"
                    .x(() => throw new InvalidOperationException("oops"))
                    .OnFailure(RemainingSteps.Run);

                "And something"
                    .x(() => { });

                "And something"
                    .x(() => { });

                "And something which fails"
                    .x(() => throw new InvalidOperationException("oops"));

                "And something"
                    .x(() => { });
            }
        }
    }
}
