// <copyright file="CollectionFixtureFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;

    public class CollectionFixtureFeature
    {
        [Background]
        public void Background()
        {
            "Given no temporary files exist"
                .f(() =>
                {
                    foreach (var path in Directory.EnumerateFiles(
                        Directory.GetCurrentDirectory(), "*.CollectionFixtureFeature"))
                    {
                        File.Delete(path);
                    }
                });
        }

        [Scenario]
        public void CollectionFixture(string collectionName, Result[] results)
        {
            "Given features with a collection fixture"
                .f(() => collectionName = "CollectionFixtureFeatureCollection");

            "When I run the scenario"
                .f(() => results = typeof(CollectionFixtureFeature).Assembly.RunScenarios(collectionName));

            "Then the class fixture is supplied as a constructor to each test class instance and disposed"
                .f(() =>
                {
                    results.Should().ContainItemsAssignableTo<Pass>();
                    ShouldBeWrittenInOrder("disposed.CollectionFixtureFeature");
                });
        }

        private static void ShouldBeWrittenInOrder(params string[] paths)
        {
            Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.CollectionFixtureFeature")
                .Select(Path.GetFileName)
                .Should().BeEquivalentTo(paths);

            var writings = paths.Select(s => new { Path = s, Ticks = Read(s) }).ToArray();
            writings.Should().Equal(writings.OrderBy(writing => writing.Ticks));
        }

        private static void Write(string path)
        {
            Thread.Sleep(1);
            using (var file = new StreamWriter(path, false))
            {
                file.Write(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static long Read(string path)
        {
            return long.Parse(File.ReadAllText(path), CultureInfo.InvariantCulture);
        }

        [CollectionDefinition("CollectionFixtureFeatureCollection")]
        public class CollectionFixtureFeatureCollection : ICollectionFixture<Fixture>
        {
        }

        [Collection("CollectionFixtureFeatureCollection")]
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
                    .f(() => this.fixture.Feature1Executed = true);
            }
        }

        [Collection("CollectionFixtureFeatureCollection")]
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
                    .f(() => this.fixture.Feature2Executed = true);
            }
        }

        public sealed class Fixture : IDisposable
        {
            public bool Feature1Executed { private get; set; }

            public bool Feature2Executed { private get; set; }

            public void Dispose()
            {
                this.Feature1Executed.Should().BeTrue();
                this.Feature2Executed.Should().BeTrue();
                Write("disposed.CollectionFixtureFeature");
            }
        }
    }
}
#endif
