// <copyright file="UsingFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

#if NET40
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to release allocated resources
    // As a developer
    // I want to register objects for disposal after a scenario has run
    public static class UsingFeature
    {
        private enum LifeTimeEventType
        {
            Constructed,
            Disposed,
        }

        [Scenario]
        public static void RegisteringManyDisposableObjectsInASingleStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a step which registers many disposable objects followed by a step which uses the objects"
                .Given(() => feature = typeof(SingleStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringDisposableObjectWhichThrowExceptionsWhenDisposed()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a step which registers disposable objects which throw exceptions when disposed followed by a step which uses the objects"
                .Given(() => feature = typeof(SingleStepWithBadDisposables));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the first n-1 results should not be failures"
                .And(() => results.Reverse().Skip(1).Should().NotContain(result => result is FailedResult));

            "And the last result should be a failure"
                .And(() => results.Reverse().First().Should().BeOfType<FailedResult>());

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringDisposableObjectsWhichRegisterAFurtherDisposableWhenDisposed()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            ("Given a step which registers disposable objects which, when disposed," +
                "throw an exception and register a further disposable objects which throw an exception when disposed")
                .Given(() => feature = typeof(SingleStepWithSingleRecursionBadDisposables));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the first n-2 results should not be failures"
                .And(() => results.Reverse().Skip(2).Should().NotContain(result => result is FailedResult));

            "And the last 2 results should be failures"
                .And(() => results.Reverse().Take(2).Should().ContainItemsAssignableTo<FailedResult>());

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => EachDisposableObjectShouldHaveBeenDisposed());
        }

        [Scenario]
        public static void RegisteringManyDisposableObjectsInSeperateSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given many steps which each register a disposable object followed by a step which uses the objects"
                .Given(() => feature = typeof(ManySteps));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringADisposableObjectInManyContexts()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a scenario with a step which registers a disposable object followed by steps which use the disposable object and generate two contexts"
                .Given(() => feature = typeof(SingleStepTwoContexts));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And two disposable objects should have been created"
                .And(() => Disposable.RecordedEvents.Count(@event => @event.EventType == LifeTimeEventType.Constructed).Should().Be(2));

            "And the disposable objects should each have been created and disposed one before the other"
                .And(() =>
                {
                    var @events = Disposable.RecordedEvents.ToArray();
                    (@events.Length % 2).Should().Be(0);
                    for (var index = 0; index < @events.Length; index = index + 2)
                    {
                        var event0 = events[index];
                        var event1 = events[index + 1];

                        event0.ObjectId.Should().Be(event1.ObjectId);
                        event0.EventType.Should().Be(LifeTimeEventType.Constructed);
                        event1.EventType.Should().Be(LifeTimeEventType.Disposed);

                        @events.Count(@event => @event.ObjectId == event0.ObjectId).Should().Be(2);
                    }
                });
        }

        [Scenario]
        public static void FailingSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a scenario with steps which register disposable objects followed by a failing step"
                .Given(() => feature = typeof(StepsFollowedByAFailingStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be one failure"
                .Then(() => results.OfType<FailedResult>().Count().Should().Be(1));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void FailureToCompleteAStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a scenario with a step which registers disposable objects but fails to complete"
                .Given(() => feature = typeof(StepFailsToComplete));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be one failure"
                .Then(() => results.OfType<FailedResult>().Count().Should().Be(1));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        private static AndConstraint<FluentAssertions.Assertions.GenericCollectionAssertions<LifetimeEvent>> SomeDisposableObjectsShouldHaveBeenCreated()
        {
            return Disposable.RecordedEvents.Where(@event => @event.EventType == LifeTimeEventType.Constructed).Should().NotBeEmpty();
        }

        private static AndConstraint<FluentAssertions.Assertions.BooleanAssertions> DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder()
        {
            return Disposable.RecordedEvents.SkipWhile(@event => @event.EventType != LifeTimeEventType.Disposed)
                .Reverse()
                .SequenceEqual(
                    Disposable.RecordedEvents.TakeWhile(@event => @event.EventType == LifeTimeEventType.Constructed),
                    new CustomEqualityComparer<LifetimeEvent>((x, y) => x.ObjectId == y.ObjectId, x => x.ObjectId))
                .Should().BeTrue();
        }

        private static void EachDisposableObjectShouldHaveBeenDisposed()
        {
            foreach (var x in Disposable.RecordedEvents.Where(@event => @event.EventType == LifeTimeEventType.Constructed).Select(@event => @event.ObjectId))
            {
                Disposable.RecordedEvents.Count(@event => @event.ObjectId == x && @event.EventType == LifeTimeEventType.Disposed).Should().Be(1);
            }
        }

        private static class SingleStep
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);
                var disposable1 = default(Disposable);
                var disposable2 = default(Disposable);

                "Given some disposables"
                    .Given(() =>
                    {
                        disposable0 = new Disposable().Using();
                        disposable1 = new Disposable().Using();
                        disposable2 = new Disposable().Using();
                    });

                "When using the disposables"
                    .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class SingleStepWithBadDisposables
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(BadDisposable);
                var disposable1 = default(BadDisposable);
                var disposable2 = default(BadDisposable);

                "Given some disposables"
                    .Given(() =>
                    {
                        disposable0 = new BadDisposable().Using();
                        disposable1 = new BadDisposable().Using();
                        disposable2 = new BadDisposable().Using();
                    });

                "When using the disposables"
                    .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class SingleStepWithSingleRecursionBadDisposables
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(SingleRecursionBadDisposable);
                var disposable1 = default(SingleRecursionBadDisposable);
                var disposable2 = default(SingleRecursionBadDisposable);

                "Given some disposables"
                    .Given(() =>
                    {
                        disposable0 = new SingleRecursionBadDisposable().Using();
                        disposable1 = new SingleRecursionBadDisposable().Using();
                        disposable2 = new SingleRecursionBadDisposable().Using();
                    });

                "When using the disposables"
                    .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class ManySteps
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);
                var disposable1 = default(Disposable);
                var disposable2 = default(Disposable);

                "Given a disposable"
                    .Given(() => disposable0 = new Disposable().Using());

                "And another disposable"
                    .Given(() => disposable1 = new Disposable().Using());

                "And another disposable"
                    .Given(() => disposable2 = new Disposable().Using());

                "When using the disposables"
                    .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class SingleStepTwoContexts
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);

                "Given a disposable"
                    .Given(() => disposable0 = new Disposable().Using());

                "When using the disposable"
                    .When(() => disposable0.Use());

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
                var disposable0 = default(Disposable);
                var disposable1 = default(Disposable);
                var disposable2 = default(Disposable);

                "Given a disposable"
                    .Given(() => disposable0 = new Disposable().Using());

                "And another disposable"
                    .Given(() => disposable1 = new Disposable().Using());

                "And another disposable"
                    .Given(() => disposable2 = new Disposable().Using());

                "When using the disposables"
                    .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });

                "Then something happens"
                    .Then(() => 1.Should().Be(0));
            }
        }

        private static class StepFailsToComplete
        {
            [Scenario]
            public static void Scenario()
            {
                "Given some disposables"
                    .Given(() =>
                    {
                        new Disposable().Using();
                        new Disposable().Using();
                        new Disposable().Using();
                        throw new InvalidOperationException();
                    });
            }
        }

        private class Disposable : IDisposable
        {
            private static readonly ConcurrentQueue<LifetimeEvent> Events = new ConcurrentQueue<LifetimeEvent>();

            private bool isDisposed;

            public Disposable()
            {
                Events.Enqueue(new LifetimeEvent { EventType = LifeTimeEventType.Constructed, ObjectId = this.GetHashCode() });
            }

            ~Disposable()
            {
                this.Dispose(false);
            }

            public static IEnumerable<LifetimeEvent> RecordedEvents
            {
                get { return Disposable.Events.Select(_ => _); }
            }

            public static void ClearRecordedEvents()
            {
                LifetimeEvent ignored;
                while (Events.TryDequeue(out ignored))
                {
                }
            }

            public void Use()
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Events.Enqueue(new LifetimeEvent { EventType = LifeTimeEventType.Disposed, ObjectId = this.GetHashCode() });
                    this.isDisposed = true;
                }
            }
        }

        private sealed class BadDisposable : Disposable
        {
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    throw new NotImplementedException();
                }
            }
        }

        private sealed class SingleRecursionBadDisposable : Disposable
        {
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    new BadDisposable().Using();
                    throw new NotImplementedException();
                }
            }
        }

        private class LifetimeEvent
        {
            public LifeTimeEventType EventType { get; set; }

            public int ObjectId { get; set; }
        }
    }
}
#endif