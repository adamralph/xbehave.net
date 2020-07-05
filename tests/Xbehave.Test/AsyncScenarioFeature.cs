namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class AsyncScenarioFeature : Feature
    {
        [Scenario]
        public void AsyncScenario(Type feature, ITestResultMessage[] results)
        {
            "Given an async scenario that throws after yielding"
                .x(() => feature = typeof(AsyncScenarioThatThrowsAfterYielding));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the scenario fails"
                .x(() => Assert.IsAssignableFrom<ITestFailed>(results.Single()));

            "And the exception is the exception thrown after the yield"
                .x(() => Assert.Equal("I yielded before this.", results.Cast<ITestFailed>().Single().Messages.Single()));
        }

        [Scenario]
        public void NullStepBody() =>
            "Given a null body"
                .x((Func<Task>)null);

        [Scenario]
        public void NullContextualStepBody() =>
            "Given a null body"
                .x((Func<IStepContext, Task>)null);

        private static class AsyncScenarioThatThrowsAfterYielding
        {
            [Scenario]
            public static async Task Scenario()
            {
                "Given"
                    .x(() => { });

                await Task.Yield();
                throw new InvalidOperationException("I yielded before this.");
            }
        }
    }
}
