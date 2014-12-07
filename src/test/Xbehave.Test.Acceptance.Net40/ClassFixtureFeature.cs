// <copyright file="ClassFixtureFeature.cs" company="xBehave.net contributors">
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

    public static class ClassFixtureFeature
    {
        [Background]
        public static void Background()
        {
            "Given no temporary files exist"
                .f(() =>
                {
                    foreach (var path in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.ClassFixtureFeature"))
                    {
                        File.Delete(path);
                    }
                });
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
                    ShouldBeWrittenInOrder("disposed.ClassFixtureFeature");
                });
        }

        private static void ShouldBeWrittenInOrder(params string[] paths)
        {
            Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.ClassFixtureFeature").Select(Path.GetFileName)
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

        private class ScenarioWithAClassFixture : IClassFixture<Foo>
        {
            private readonly Foo foo;

            public ScenarioWithAClassFixture(Foo foo)
            {
                foo.Should().NotBeNull();
                this.foo = foo;
            }

            [Scenario]
            public void Scenario1()
            {
                "Given"
                    .f(() => this.foo.Scenario1Executed = true);
            }

            [Scenario]
            public void Scenario2()
            {
                "Given"
                    .f(() => this.foo.Scenario2Executed = true);
            }
        }

        private sealed class Foo : IDisposable
        {
            public bool Scenario1Executed { private get; set; }

            public bool Scenario2Executed { private get; set; }

            public void Dispose()
            {
                this.Scenario1Executed.Should().BeTrue();
                this.Scenario2Executed.Should().BeTrue();
                Write("disposed.ClassFixtureFeature");
            }
        }
    }
}
#endif
