namespace Xbehave.Test
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class AsyncCollectionFixtureFeature : Feature
    {
        [Background]
        public void Background() =>
            "Given no events have occurred"
                .x(() => typeof(AsyncCollectionFixtureFeature).ClearTestEvents());

        [Scenario]
        public void AsyncCollectionFixture(string collectionName, ITestResultMessage[] results)
        {
            "Given features with an async collection fixture"
                .x(() => collectionName = "AsyncCollectionFixtureTestFeatures");

            "When I run the features"
                .x(() => results = this.Run<ITestResultMessage>(
                    typeof(AsyncCollectionFixtureFeature).GetTypeInfo().Assembly, collectionName));

            "Then the collection fixture is initialized, supplied as a constructor to each test class instance, and disposed"
                .x(() =>
                {
                    Assert.All(results, result => Assert.IsAssignableFrom<ITestPassed>(result));
                    Assert.Collection(
                        typeof(AsyncCollectionFixtureFeature).GetTestEvents(),
                        @event => Assert.Equal("initialized", @event),
                        @event => Assert.Equal("disposed", @event));
                });
        }

        [CollectionDefinition("AsyncCollectionFixtureTestFeatures")]
        public class AsyncCollectionFixtureTestFeatures : ICollectionFixture<AsyncFixture>
        {
        }

        [Collection("AsyncCollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture1
        {
            private readonly AsyncFixture fixture;

            public ScenarioWithACollectionFixture1(AsyncFixture fixture)
            {
                Assert.NotNull(fixture);
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1() =>
                "Given"
                    .x(() => this.fixture.Feature1Executed());
        }

        [Collection("AsyncCollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture2
        {
            private readonly AsyncFixture fixture;

            public ScenarioWithACollectionFixture2(AsyncFixture fixture)
            {
                Assert.NotNull(fixture);
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1() =>
                "Given"
                    .x(() => this.fixture.Feature2Executed());
        }

        public sealed class AsyncFixture : IAsyncLifetime
        {
            private bool feature1Executed;
            private bool feature2Executed;

            public void Feature1Executed() => this.feature1Executed = true;

            public void Feature2Executed() => this.feature2Executed = true;

            public Task InitializeAsync()
            {
                Assert.False(this.feature1Executed);
                Assert.False(this.feature2Executed);
                typeof(AsyncCollectionFixtureFeature).SaveTestEvent("initialized");
                return Task.CompletedTask;
            }

            public Task DisposeAsync()
            {
                Assert.True(this.feature1Executed);
                Assert.True(this.feature2Executed);
                typeof(AsyncCollectionFixtureFeature).SaveTestEvent("disposed");
                return Task.CompletedTask;
            }
        }
    }
}
