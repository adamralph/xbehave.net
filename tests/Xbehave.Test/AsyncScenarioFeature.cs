namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave.Sdk;
    using Xbehave.Test.Infrastructure;
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
                .x(() => results.Single().Should().BeAssignableTo<ITestFailed>());

            "And the exception is the exception thrown after the yield"
                .x(() => results.Cast<ITestFailed>().Single().Messages.Single().Should().Be("I yielded before this."));
        }

        [Scenario]
        public void NullStepBody() =>
            "Given a null body"
                .x(default(Func<Task>));

        [Scenario]
        public void NullContextualStepBody() =>
            "Given a null body"
                .x(default(Func<IStepContext, Task>));

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
