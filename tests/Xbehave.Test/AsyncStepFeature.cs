namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Test.Infrastructure;
    using Xunit.Abstractions;

    public class AsyncStepFeature : Feature
    {
        [Scenario]
        public void AsyncTaskStepThrowsException(Type feature, ITestResultMessage[] results)
        {
            "Given an async step that throws after yielding"
                .x(() => feature = typeof(AsyncStepWhichThrowsAfterYielding));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the step fails"
                .x(() => results.Single().Should().BeAssignableTo<ITestFailed>());

            "And the exception is the exception thrown after the yield"
                .x(() => results.Cast<ITestFailed>().Single().Messages.Single().Should().Be("I yielded before this."));
        }

        [Scenario]
        public void AsyncVoidStepThrowsException(Type feature, ITestResultMessage[] results)
        {
            "Given an async void step that throws after yielding"
                .x(() => feature = typeof(AsyncVoidStepWhichThrowsAfterYielding));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the step fails"
                .x(() => results.Single().Should().BeAssignableTo<ITestFailed>());

            "And the exception is the exception thrown after the yield"
                .x(() => results.Cast<ITestFailed>().Single().Messages.Single().Should().Be("I yielded before this."));
        }

        private static class AsyncStepWhichThrowsAfterYielding
        {
            [Scenario]
            public static void Scenario() =>
                "Given something"
                    .x(async () =>
                    {
                        await Task.Yield();
                        throw new InvalidOperationException("I yielded before this.");
                    });
        }

        private static class AsyncVoidStepWhichThrowsAfterYielding
        {
            [Scenario]
            public static void Scenario() =>
                "Given something"
                    .x(Step);

            private static async void Step()
            {
                await Task.Yield();
                throw new InvalidOperationException("I yielded before this.");
            }
        }
    }
}
