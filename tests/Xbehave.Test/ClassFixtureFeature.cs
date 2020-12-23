using System;
using Xbehave.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test
{
    public class ClassFixtureFeature : Feature
    {
        [Background]
        public void Background() =>
            "Given no events have occurred"
                .x(() => typeof(ClassFixtureFeature).ClearTestEvents());

        [Scenario]
        public void ClassFixture(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a class fixture"
                .x(() => feature = typeof(ScenarioWithAClassFixture));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the class fixture is supplied as a constructor to each test class instance and disposed"
                .x(() =>
                {
                    Assert.All(results, result => Assert.IsAssignableFrom<ITestPassed>(result));
                    Assert.All(typeof(ClassFixtureFeature).GetTestEvents(), @event => Assert.Equal("disposed", @event));
                });
        }

        private class ScenarioWithAClassFixture : IClassFixture<Fixture>
        {
            private readonly Fixture fixture;

            public ScenarioWithAClassFixture(Fixture fixture)
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

        private sealed class Fixture : IDisposable
        {
            public bool Scenario1Executed { private get; set; }

            public bool Scenario2Executed { private get; set; }

            public void Dispose()
            {
                Assert.True(this.Scenario1Executed);
                Assert.True(this.Scenario2Executed);
                typeof(ClassFixtureFeature).SaveTestEvent("disposed");
            }
        }
    }
}
