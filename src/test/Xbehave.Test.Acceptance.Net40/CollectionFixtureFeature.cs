// <copyright file="CollectionFixtureFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;

    public static class CollectionFixtureFeature
    {
        [Background]
         public static void Background()
        {
            "Given no events have occurred"
                .f(() => typeof(CollectionFixtureFeature).ClearTestEvents());
        }

        [Scenario]
        public static void CollectionFixture(string collectionName, Result[] results)
        {
            "Given features with a collection fixture"
                .f(() => collectionName = "CollectionFixtureTestFeatures");

            "When I run the features"
                .f(() => results = typeof(CollectionFixtureFeature).Assembly.RunScenarios(collectionName));

            "Then the collection fixture is supplied as a constructor to each test class instance and disposed"
                .f(() =>
                {
                    results.Should().ContainItemsAssignableTo<Pass>();
                    typeof(CollectionFixtureFeature).GetTestEvents().Should().Equal("disposed");
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
                fixture.Should().NotBeNull();
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1()
            {
                "Given"
                    .f(() => this.fixture.Feature1Executed());
            }
        }

        [Collection("CollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture2
        {
            private readonly Fixture fixture;

            public ScenarioWithACollectionFixture2(Fixture fixture)
            {
                fixture.Should().NotBeNull();
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1()
            {
                "Given"
                    .f(() => this.fixture.Feature2Executed());
            }
        }

        public sealed class Fixture : IDisposable
        {
            private bool feature1Executed;
            private bool feature2Executed;

            public void Feature1Executed()
            {
                this.feature1Executed = true;
            }

            public void Feature2Executed()
            {
                this.feature2Executed = true;
            }

            public void Dispose()
            {
                this.feature1Executed.Should().BeTrue();
                this.feature2Executed.Should().BeTrue();
                typeof(CollectionFixtureFeature).SaveTestEvent("disposed");
            }
        }
    }
}
#endif
