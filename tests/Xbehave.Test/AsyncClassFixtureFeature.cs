namespace Xbehave.Test
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class AsyncClassFixtureFeature : Feature
    {
        [Background]
        public void Background() =>
            "Given no events have occurred"
                .x(() => typeof(ClassFixtureFeature).ClearTestEvents());

        [Scenario]
        public void AsyncClassFixture(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with an async class fixture"
                .x(() => feature = typeof(ScenarioWithAnAsyncClassFixture));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the class fixture is initialized, supplied as a constructor to each test class instance, and disposed"
                .x(() =>
                {
                    Assert.All(results, result => Assert.IsAssignableFrom<ITestPassed>(result));
                    Assert.Collection(
                        typeof(ScenarioWithAnAsyncClassFixture).GetTestEvents(),
                        @event => Assert.Equal("initialized", @event),
                        @event => Assert.Equal("disposed", @event));
                });
        }

        private class ScenarioWithAnAsyncClassFixture : IClassFixture<Fixture>
        {
            private readonly Fixture fixture;

            public ScenarioWithAnAsyncClassFixture(Fixture fixture)
            {
                Assert.NotNull(fixture);
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1() =>
                "Given"
                    .x(() => this.fixture.Scenario1Executed = true);

            [Scenario]
            public void Scenario2() =>
                "Given"
                    .x(() => this.fixture.Scenario2Executed = true);
        }

        private sealed class Fixture : IAsyncLifetime
        {
            public bool Scenario1Executed { private get; set; }

            public bool Scenario2Executed { private get; set; }

            public Task InitializeAsync()
            {
                Assert.False(this.Scenario1Executed);
                Assert.False(this.Scenario2Executed);
                typeof(AsyncCollectionFixtureFeature).SaveTestEvent("initialized");
                return Task.CompletedTask;
            }

            public Task DisposeAsync()
            {
                Assert.True(this.Scenario1Executed);
                Assert.True(this.Scenario2Executed);
                typeof(AsyncCollectionFixtureFeature).SaveTestEvent("disposed");
                return Task.CompletedTask;
            }
        }
    }
}
