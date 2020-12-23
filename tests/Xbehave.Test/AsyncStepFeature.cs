using System;
using System.Linq;
using System.Threading.Tasks;
using Xbehave;
using Xbehave.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test
{
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
                .x(() => Assert.IsAssignableFrom<ITestFailed>(results.Single()));

            "And the exception is the exception thrown after the yield"
                .x(() => Assert.Equal("I yielded before this.", results.Cast<ITestFailed>().Single().Messages.Single()));
        }

        [Scenario]
        public void AsyncVoidStepThrowsException(Type feature, ITestResultMessage[] results)
        {
            "Given an async void step that throws after yielding"
                .x(() => feature = typeof(AsyncVoidStepWhichThrowsAfterYielding));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the step fails"
                .x(() => Assert.IsAssignableFrom<ITestFailed>(results.Single()));

            "And the exception is the exception thrown after the yield"
                .x(() => Assert.Equal("I yielded before this.", results.Cast<ITestFailed>().Single().Messages.Single()));
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
