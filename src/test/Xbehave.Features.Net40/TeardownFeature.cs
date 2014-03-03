// <copyright file="TeardownFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if NET40 || NET45
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to release allocated resources
    // As a developer
    // I want to execute teardown actions after a scenario has run
    public static class TeardownFeature
    {
        private static readonly ConcurrentQueue<int> ActionIds = new ConcurrentQueue<int>();

        [Scenario]
        public static void RegisteringManyTeardownsInASingleStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a step which registers many teardowns"
                .Given(() => feature = typeof(SingleStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(TeardownFeature.ClearActionIds);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some teardowns should have been executed"
                .And(() => ActionIds.Count.Should().NotBe(0));

            "And the teardown actions should have been executed once in reverse order"
                .And(() => EachTeardownActionShouldHaveBeenExecutedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringTeardownActionsWhichThrowExceptionsWhenExecuted()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a step which registers teardown actions which throw exceptions when executed"
                .Given(() => feature = typeof(SingleStepWithBadTeardowns));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(TeardownFeature.ClearActionIds);

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the first n-1 results should not be failures"
                .And(() => results.Reverse().Skip(1).Should().NotContain(result => result is FailedResult));

            "And the last result should be a failure"
                .And(() => results.Reverse().First().Should().BeOfType<FailedResult>());

            "And some teardowns should have been executed"
                .And(() => ActionIds.Count.Should().NotBe(0));

            "And the teardown actions should have been executed once in reverse order"
                .And(() => EachTeardownActionShouldHaveBeenExecutedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringManyTeardownActionsInManySteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given many steps which each register many teardown actions"
                .Given(() => feature = typeof(ManySteps));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(TeardownFeature.ClearActionIds);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some teardowns should have been executed"
                .And(() => ActionIds.Count.Should().NotBe(0));

            "And the teardown actions should have been executed once in reverse order"
                .And(() => EachTeardownActionShouldHaveBeenExecutedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringATeardownInManyContexts()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a scenario with a step which registers a teardown action followed by steps which generate two contexts"
                .Given(() => feature = typeof(SingleStepTwoContexts));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(TeardownFeature.ClearActionIds);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And two teardown actions should have been executed"
                .And(() => ActionIds.Count.Should().Be(2));
        }

        [Scenario]
        public static void FailingSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a scenario with steps which register teardown actions followed by a failing step"
                .Given(() => feature = typeof(StepsFollowedByAFailingStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(TeardownFeature.ClearActionIds);

            "Then there should be one failure"
                .Then(() => results.OfType<FailedResult>().Count().Should().Be(1));

            "And some teardowns should have been executed"
                .And(() => ActionIds.Count.Should().NotBe(0));

            "And the teardown actions should have been executed once in reverse order"
                .And(() => EachTeardownActionShouldHaveBeenExecutedOnceInReverseOrder());
        }

        [Scenario]
        public static void FailureToCompleteAStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a scenario with a step which registers teardown actions but fails to complete"
                .Given(() => feature = typeof(StepFailsToComplete));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(TeardownFeature.ClearActionIds);

            "Then there should be one failure"
                .Then(() => results.OfType<FailedResult>().Count().Should().Be(1));

            "And some teardowns should have been executed"
                .And(() => ActionIds.Count.Should().NotBe(0));

            "And the teardown actions should have been executed once in reverse order"
                .And(() => EachTeardownActionShouldHaveBeenExecutedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringDisposableObjectsAndTeardownActions()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given steps which each register disposable objects and teardown actions"
                .Given(() => feature = typeof(TeardownsAndDisposables));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(TeardownFeature.ClearActionIds);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some teardowns should have been executed"
                .And(() => ActionIds.Count.Should().NotBe(0));

            "And the teardown actions should have been executed once in reverse order"
                .And(() => EachTeardownActionShouldHaveBeenExecutedOnceInReverseOrder());
        }

        private static void ClearActionIds()
        {
            int ignored;
            while (ActionIds.TryDequeue(out ignored))
            {
            }
        }

        private static int EachTeardownActionShouldHaveBeenExecutedOnceInReverseOrder()
        {
            return ActionIds.Aggregate(
                ActionIds.First() + 1,
                (previous, current) =>
                {
                    current.Should().Be(previous - 1);
                    return current;
                });
        }

        private static class SingleStep
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { })
                    .Teardown(() => ActionIds.Enqueue(1))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(2))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(3));
            }
        }

        private static class SingleStepWithBadTeardowns
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { })
                    .Teardown(() =>
                    {
                        ActionIds.Enqueue(1);
                        throw new InvalidOperationException();
                    })
                    .And()
                    .Teardown(() =>
                    {
                        ActionIds.Enqueue(2);
                        throw new InvalidOperationException();
                    })
                    .And()
                    .Teardown(() =>
                    {
                        ActionIds.Enqueue(3);
                        throw new InvalidOperationException();
                    });
            }
        }

        private static class ManySteps
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { })
                    .Teardown(() => ActionIds.Enqueue(1))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(2))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(3));

                "And something else"
                    .And(() => { })
                    .Teardown(() => ActionIds.Enqueue(4))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(5))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(6));
            }
        }

        private static class SingleStepTwoContexts
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { })
                    .Teardown(() => ActionIds.Enqueue(1));

                "When something happens"
                    .When(() => { });

                "Then something"
                    .Then(() => { })
                    .InIsolation();

                "And something"
                    .Then(() => { });
            }
        }

        private static class StepsFollowedByAFailingStep
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() => { })
                    .Teardown(() => ActionIds.Enqueue(1));

                "And something"
                    .And(() => { })
                    .Teardown(() => ActionIds.Enqueue(2));

                "And something"
                    .And(() => { })
                    .Teardown(() => ActionIds.Enqueue(3));

                "When something happens"
                    .When(() => { });

                "Then something happens"
                    .Then(() => 1.Should().Be(0));
            }
        }

        private static class StepFailsToComplete
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(() =>
                    {
                        throw new InvalidOperationException();
                    })
                    .Teardown(() => ActionIds.Enqueue(1))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(2))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(3));
            }
        }

        private static class TeardownsAndDisposables
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .Given(c =>
                    {
                        new Disposable(1).Using(c);
                        new Disposable(2).Using(c);
                        new Disposable(3).Using(c);
                    })
                    .Teardown(() => ActionIds.Enqueue(4))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(5))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(6));

                "And something else"
                    .And(c =>
                    {
                        new Disposable(7).Using(c);
                        new Disposable(8).Using(c);
                        new Disposable(9).Using(c);
                    })
                    .Teardown(() => ActionIds.Enqueue(10))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(11))
                    .And()
                    .Teardown(() => ActionIds.Enqueue(12));
            }
        }

        private class Disposable : IDisposable
        {
            private readonly int id;

            public Disposable(int id)
            {
                this.id = id;
            }

            public void Dispose()
            {
                ActionIds.Enqueue(this.id);
            }
        }
    }
}
#endif
