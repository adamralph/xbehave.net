// <copyright file="ObjectDisposalFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if NET40 || NET45
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
#if NET45
    using System.Threading.Tasks;
#endif
    using FluentAssertions;
    using Xbehave.Features.Infrastructure;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to release allocated resources
    // As a developer
    // I want to register objects for disposal after a scenario has run
    public static class ObjectDisposalFeature
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
            var results = default(Result[]);

            "Given a step which registers many disposable objects followed by a step which uses the objects"
                .Given(() => feature = typeof(SingleStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is Fail));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringManyDisposableObjectsInASingleStepWithATimeout()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a step which registers many disposable objects followed by a step which uses the objects"
                .Given(() => feature = typeof(SingleStepWithATimeout));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is Fail));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringDisposableObjectWhichThrowExceptionsWhenDisposed()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a step which registers disposable objects which throw exceptions when disposed followed by a step which uses the objects"
                .Given(() => feature = typeof(SingleStepWithBadDisposables));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the first n-1 results should not be failures"
                .And(() => results.Reverse().Skip(1).Should().NotContain(result => result is Fail));

            "And the last result should be a failure"
                .And(() => results.Reverse().First().Should().BeOfType<Fail>());

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

#if !V2
        [Scenario]
        public static void RegisteringDisposableObjectsWhichRegisterAFurtherDisposableWhenDisposed()
        {
            var feature = default(Type);
            var results = default(Result[]);

            ("Given a step which registers disposable objects which, when disposed," +
                "throw an exception and register a further disposable objects which throw an exception when disposed")
                .Given(() => feature = typeof(SingleStepWithSingleRecursionBadDisposables));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then the results should not be empty"
                .Then(() => results.Should().NotBeEmpty());

            "And the first n-2 results should not be failures"
                .And(() => results.Reverse().Skip(2).Should().NotContain(result => result is Fail));

            "And the last 2 results should be failures"
                .And(() => results.Reverse().Take(2).Should().ContainItemsAssignableTo<Fail>());

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => EachDisposableObjectShouldHaveBeenDisposed());
        }

#endif
        [Scenario]
        public static void RegisteringManyDisposableObjectsInSeparateSteps()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given many steps which each register a disposable object followed by a step which uses the objects"
                .Given(() => feature = typeof(ManySteps));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is Fail));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringADisposableObjectInManyContexts()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a scenario with a step which registers a disposable object followed by steps which use the disposable object and generate two contexts"
                .Given(() => feature = typeof(SingleStepTwoContexts));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is Fail));

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
            var results = default(Result[]);

            "Given a scenario with steps which register disposable objects followed by a failing step"
                .Given(() => feature = typeof(StepsFollowedByAFailingStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be one failure"
                .Then(() => results.OfType<Fail>().Count().Should().Be(1));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void FailureToCompleteAStep()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a scenario with a step which registers disposable objects but fails to complete"
                .Given(() => feature = typeof(StepFailsToComplete));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be one failure"
                .Then(() => results.OfType<Fail>().Count().Should().Be(1));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

#if NET45
        [Scenario]
        public static void RegisteringManyDisposableObjectsInAnAsyncStep()
        {
            var feature = default(Type);
            var results = default(Result[]);

            "Given a step which registers many disposable objects in an async step"
                .Given(() => feature = typeof(AsyncStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(Disposable.ClearRecordedEvents);

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is Fail));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }
#endif

        private static AndConstraint<FluentAssertions.Collections.GenericCollectionAssertions<LifetimeEvent>> SomeDisposableObjectsShouldHaveBeenCreated()
        {
            return Disposable.RecordedEvents.Where(@event => @event.EventType == LifeTimeEventType.Constructed).Should().NotBeEmpty();
        }

        private static AndConstraint<FluentAssertions.Primitives.BooleanAssertions> DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder()
        {
            return Disposable.RecordedEvents.SkipWhile(@event => @event.EventType != LifeTimeEventType.Disposed)
                .Reverse()
                .SequenceEqual(
                    Disposable.RecordedEvents.TakeWhile(@event => @event.EventType == LifeTimeEventType.Constructed),
                    new CustomEqualityComparer<LifetimeEvent>((x, y) => x.ObjectId == y.ObjectId, x => x.ObjectId.GetHashCode()))
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
                    .Given(c =>
                    {
                        disposable0 = new Disposable().Using(c);
                        disposable1 = new Disposable().Using(c);
                        disposable2 = new Disposable().Using(c);
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

        private static class SingleStepWithATimeout
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);
                var disposable1 = default(Disposable);
                var disposable2 = default(Disposable);

                "Given some disposables"
                    .Given(c =>
                    {
                        disposable0 = new Disposable().Using(c);
                        disposable1 = new Disposable().Using(c);
                        disposable2 = new Disposable().Using(c);
                    })
                    .WithTimeout(Timeout.Infinite);

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
                    .Given(c =>
                    {
                        disposable0 = new BadDisposable().Using(c);
                        disposable1 = new BadDisposable().Using(c);
                        disposable2 = new BadDisposable().Using(c);
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

#if !V2
        private static class SingleStepWithSingleRecursionBadDisposables
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(SingleRecursionBadDisposable);
                var disposable1 = default(SingleRecursionBadDisposable);
                var disposable2 = default(SingleRecursionBadDisposable);

                "Given some disposables"
                    .Given(c =>
                    {
                        disposable0 = new SingleRecursionBadDisposable().Using(c);
                        disposable1 = new SingleRecursionBadDisposable().Using(c);
                        disposable2 = new SingleRecursionBadDisposable().Using(c);
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

#endif
        private static class ManySteps
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);
                var disposable1 = default(Disposable);
                var disposable2 = default(Disposable);

                "Given a disposable"
                    .Given(c => disposable0 = new Disposable().Using(c));

                "And another disposable"
                    .Given(c => disposable1 = new Disposable().Using(c));

                "And another disposable"
                    .Given(c => disposable2 = new Disposable().Using(c));

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
                    .Given(c => disposable0 = new Disposable().Using(c));

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
                    .Given(c => disposable0 = new Disposable().Using(c));

                "And another disposable"
                    .Given(c => disposable1 = new Disposable().Using(c));

                "And another disposable"
                    .Given(c => disposable2 = new Disposable().Using(c));

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
                    .Given(c =>
                    {
                        new Disposable().Using(c);
                        new Disposable().Using(c);
                        new Disposable().Using(c);
                        throw new InvalidOperationException();
                    });
            }
        }

#if NET45
        private static class AsyncStep
        {
            [Scenario]
            public static void Scenario(Disposable disposable0, Disposable disposable1, Disposable disposable2)
            {
                "Given some disposables"
                    .Given(async c =>
                    {
                        await Task.Yield();
                        disposable0 = new Disposable().Using(c);
                        disposable1 = new Disposable().Using(c);
                        disposable2 = new Disposable().Using(c);
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
#endif

        private class Disposable : IDisposable
        {
            private static readonly ConcurrentQueue<LifetimeEvent> Events = new ConcurrentQueue<LifetimeEvent>();
            private readonly Guid id = Guid.NewGuid();
            private bool isDisposed;

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Private class.")]
            public Disposable()
            {
                Events.Enqueue(new LifetimeEvent { EventType = LifeTimeEventType.Constructed, ObjectId = this.id });
            }

            ~Disposable()
            {
                this.Dispose(false);
            }

            public static IEnumerable<LifetimeEvent> RecordedEvents
            {
                get { return Events.Select(_ => _); }
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
                    Events.Enqueue(new LifetimeEvent { EventType = LifeTimeEventType.Disposed, ObjectId = this.id });
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

#if !V2
        private sealed class SingleRecursionBadDisposable : Disposable
        {
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
#pragma warning disable 618
                    new BadDisposable().Using();
#pragma warning restore 618
                    throw new NotImplementedException();
                }
            }
        }

#endif
        private class LifetimeEvent
        {
            public LifeTimeEventType EventType { get; set; }

            public Guid ObjectId { get; set; }
        }
    }
}
#endif