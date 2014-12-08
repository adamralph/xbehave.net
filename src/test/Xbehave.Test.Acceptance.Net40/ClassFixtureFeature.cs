// <copyright file="ClassFixtureFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;

    public static class ClassFixtureFeature
    {
        [Background]
        public static void Background()
        {
            "Given no events have occurred"
                .f(() => typeof(ClassFixtureFeature).ClearTestEvents());
        }

        [Scenario]
        public static void ClassFixture(Type feature, Result[] results)
        {
            "Given a scenario with a class fixture"
                .f(() => feature = typeof(ScenarioWithAClassFixture));

            "When I run the scenario"
                .f(() => results = feature.RunScenarios());

            "Then the class fixture is supplied as a constructor to each test class instance and disposed"
                .f(() =>
                {
                    results.Should().ContainItemsAssignableTo<Pass>();
                    typeof(ClassFixtureFeature).GetTestEvents().Should().Equal("disposed");
                });
        }

        private class ScenarioWithAClassFixture : IClassFixture<Fixture>
        {
            private readonly Fixture fixture;

            public ScenarioWithAClassFixture(Fixture fixture)
            {
                fixture.Should().NotBeNull();
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1()
            {
                "Given"
                    .f(() => this.fixture.Scenario1Executed = true);
            }

            [Scenario]
            public void Scenario2()
            {
                "Given"
                    .f(() => this.fixture.Scenario2Executed = true);
            }
        }

        private sealed class Fixture : IDisposable
        {
            public bool Scenario1Executed { private get; set; }

            public bool Scenario2Executed { private get; set; }

            public void Dispose()
            {
                this.Scenario1Executed.Should().BeTrue();
                this.Scenario2Executed.Should().BeTrue();
                typeof(ClassFixtureFeature).SaveTestEvent("disposed");
            }
        }
    }
}
#endif
