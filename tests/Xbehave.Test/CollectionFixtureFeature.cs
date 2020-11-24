namespace Xbehave.Test
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xbehave.Test.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class CollectionFixtureFeature : Feature
    {
        [Background]
        public void Background() =>
            "Given no events have occurred"
                .x(() => typeof(CollectionFixtureFeature).ClearTestEvents());

        [Scenario]
        public void CollectionFixture(string collectionName, ITestResultMessage[] results)
        {
            "Given features with a collection fixture"
                .x(() => collectionName = "CollectionFixtureTestFeatures");

            "When I run the features"
                .x(() => results = this.Run<ITestResultMessage>(
                    typeof(CollectionFixtureFeature).GetTypeInfo().Assembly, collectionName));

            "Then the collection fixture is supplied as a constructor to each test class instance and disposed"
                .x(() =>
                {
                    Assert.All(results, result => Assert.IsAssignableFrom<ITestPassed>(result));
                    Assert.All(typeof(CollectionFixtureFeature).GetTestEvents(), @event => Assert.Equal("disposed", @event));
                });
        }

        [CollectionDefinition("CollectionFixtureTestFeatures")]
        public class CollectionFixtureTestFeatures : ICollectionFixture<Fixture>
        {
        }

        [Collection("CollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture1
        {
            private readonly Fixture fixture;

            public ScenarioWithACollectionFixture1(Fixture fixture)
            {
                Assert.NotNull(fixture);
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1() =>
                "Given"
                    .x(() => this.fixture.Feature1Executed());
        }

        [Collection("CollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture2
        {
            private readonly Fixture fixture;

            public ScenarioWithACollectionFixture2(Fixture fixture)
            {
                Assert.NotNull(fixture);
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1() =>
                "Given"
                    .x(() => this.fixture.Feature2Executed());
        }

        public sealed class Fixture : IDisposable
        {
            private bool feature1Executed;
            private bool feature2Executed;

            public void Feature1Executed() => this.feature1Executed = true;

            public void Feature2Executed() => this.feature2Executed = true;

            public void Dispose()
            {
                Assert.True(this.feature1Executed);
                Assert.True(this.feature2Executed);
                typeof(CollectionFixtureFeature).SaveTestEvent("disposed");
            }
        }

        [CollectionDefinition(nameof(CollectionFixtureAsyncLifecycleForScenario))]
        public class CollectionFixtureAsyncLifecycleForScenario : ICollectionFixture<AsyncFixtureForScenario>
        {
        }

        [Collection(nameof(CollectionFixtureAsyncLifecycleForScenario))]
        public class ScenarioWithUnusedAsyncCollectionFixture
        {
            [Scenario]
            public void Scenario1() =>
                "Given"
                    .x(() =>
                       {
                           Assert.NotNull(AsyncFixtureForScenario.Instance);
                           Assert.True(AsyncFixtureForScenario.Instance.InitializeAsyncExecuted);
                       });
        }

        public sealed class AsyncFixtureForScenario : AsyncFixture<AsyncFixtureForScenario> { }

        [CollectionDefinition(nameof(CollectionFixtureAsyncLifecycleForFact))]
        public class CollectionFixtureAsyncLifecycleForFact : ICollectionFixture<AsyncFixtureForFact>
        {
        }

        [Collection(nameof(CollectionFixtureAsyncLifecycleForFact))]
        public class FactWithUnusedAsyncCollectionFixture
        {
            [Fact]
            public void Fact1()
            {
                Assert.NotNull(AsyncFixtureForFact.Instance);
                Assert.True(AsyncFixtureForFact.Instance.InitializeAsyncExecuted);
            }
        }

        public sealed class AsyncFixtureForFact : AsyncFixture<AsyncFixtureForFact> {}

        public class AsyncFixture<TFixture> : IAsyncLifetime, IDisposable
        where TFixture : AsyncFixture<TFixture>
        {
            public static TFixture Instance;

            public AsyncFixture() => Instance = (TFixture)this;

            public bool InitializeAsyncExecuted { get; private set; }

            public bool DisposeAsyncExecuted { get; private set; }

            public Task InitializeAsync()
            {
                this.InitializeAsyncExecuted = true;
                return Task.CompletedTask;
            }

            public Task DisposeAsync()
            {
                this.DisposeAsyncExecuted = true;
                return Task.CompletedTask;
            }

            public virtual void Dispose()
            {
                Assert.True(this.InitializeAsyncExecuted);
                Assert.True(this.DisposeAsyncExecuted);
                Instance = null;
            }
        }
    }
}
