// <copyright file="ObjectDisposalFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave.Test.Infrastructure;
    using Xunit.Abstractions;

    // In order to release allocated resources
    // As a developer
    // I want to register objects for disposal after a scenario has run
    public class ObjectDisposalFeature : Feature
    {
        [Background]
        public void Background()
        {
            "Given no events have occurred"
                .x(() => typeof(ObjectDisposalFeature).ClearTestEvents());
        }

        [Scenario]
        [Example(typeof(AStepWithThreeDisposables))]
        [Example(typeof(ThreeStepsWithDisposables))]
        [Example(typeof(AnAsyncStepWithThreeDisposables))]
        public void ManyDisposablesInASingleStep(Type feature, ITestResultMessage[] results)
        {
            "Given {0}"
                .x(() => { });

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "And there should be no failures"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());

            "And the disposables should each have been disposed in reverse order"
                .x(() => typeof(ObjectDisposalFeature).GetTestEvents()
                    .Should().Equal("disposed3", "disposed2", "disposed1"));
        }

        [Scenario]
        public void ADisposableWhichThrowExceptionsWhenDisposed(Type feature, ITestResultMessage[] results)
        {
            "Given a step with three disposables which throw exceptions when disposed"
                .x(() => feature = typeof(StepWithThreeBadDisposables));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the there should be at least two results"
                .x(() => results.Length.Should().BeGreaterOrEqualTo(2));

            "And the first n-1 results should be passes"
                .x(() => results.Reverse().Skip(1).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the last result should be a failure"
                .x(() => results.Reverse().First().Should().BeAssignableTo<ITestFailed>());

            "And the disposables should be disposed in reverse order"
                .x(() => typeof(ObjectDisposalFeature).GetTestEvents()
                    .Should().Equal("disposed3", "disposed2", "disposed1"));
        }

        [Scenario]
        [Example(typeof(StepsFollowedByAFailingStep))]
        [Example(typeof(StepFailsToComplete))]
        public void FailingSteps(Type feature, ITestResultMessage[] results)
        {
            "Given {0}"
                .x(() => { });

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one failure"
                .x(() => results.OfType<ITestFailed>().Count().Should().Be(1));

            "And the disposables should be disposed in reverse order"
                .x(() => typeof(ObjectDisposalFeature).GetTestEvents()
                    .Should().Equal("disposed3", "disposed2", "disposed1"));
        }

        [Scenario]
        public void DisposablesAndTeardowns(Type feature, ITestResultMessage[] results)
        {
            "Given steps with disposables and teardowns"
                .x(() => feature = typeof(StepsWithDisposablesAndTeardowns));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "And there should be no failures"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());

            "And the disposables and teardowns should be disposed/executed in reverse order"
                .x(() => typeof(ObjectDisposalFeature).GetTestEvents()
                    .Should().Equal("teardown4", "disposed3", "teardown2", "disposed1"));
        }

        [Scenario]
        public void NullDisposable()
        {
            "Given a null body"
                .x(c => ((IDisposable)null).Using(c));
        }

        private static class AStepWithThreeDisposables
        {
            [Scenario]
            public static void Scenario(
                Disposable disposable0, Disposable disposable1, Disposable disposable2)
            {
                "Given some disposables"
                    .x(c =>
                    {
                        disposable0 = new Disposable(1).Using(c);
                        disposable1 = new Disposable(2).Using(c);
                        disposable2 = new Disposable(3).Using(c);
                    });

                "When using the disposables"
                    .x(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class StepWithThreeBadDisposables
        {
            [Scenario]
            public static void Scenario(
                Disposable disposable0, Disposable disposable1, Disposable disposable2)
            {
                "Given some disposables"
                    .x(c =>
                    {
                        disposable0 = new BadDisposable(1).Using(c);
                        disposable1 = new BadDisposable(2).Using(c);
                        disposable2 = new BadDisposable(3).Using(c);
                    });

                "When using the disposables"
                    .x(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class ThreeStepsWithDisposables
        {
            [Scenario]
            public static void Scenario(
                Disposable disposable0, Disposable disposable1, Disposable disposable2)
            {
                "Given a disposable"
                    .x(c => disposable0 = new Disposable(1).Using(c));

                "And another disposable"
                    .x(c => disposable1 = new Disposable(2).Using(c));

                "And another disposable"
                    .x(c => disposable2 = new Disposable(3).Using(c));

                "When using the disposables"
                    .x(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class StepsFollowedByAFailingStep
        {
            [Scenario]
            public static void Scenario(
                Disposable disposable0, Disposable disposable1, Disposable disposable2)
            {
                "Given a disposable"
                    .x(c => disposable0 = new Disposable(1).Using(c));

                "And another disposable"
                    .x(c => disposable1 = new Disposable(2).Using(c));

                "And another disposable"
                    .x(c => disposable2 = new Disposable(3).Using(c));

                "When using the disposables"
                    .x(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });

                "Then something happens"
                    .x(() => 1.Should().Be(0));
            }
        }

        private static class StepFailsToComplete
        {
            [Scenario]
            public static void Scenario()
            {
                "Given some disposables"
                    .x(c =>
                    {
                        new Disposable(1).Using(c);
                        new Disposable(2).Using(c);
                        new Disposable(3).Using(c);
                        throw new InvalidOperationException();
                    });
            }
        }

        private static class AnAsyncStepWithThreeDisposables
        {
            [Scenario]
            public static void Scenario(
                Disposable disposable0, Disposable disposable1, Disposable disposable2)
            {
                "Given some disposables"
                    .x(async c =>
                    {
                        await Task.Yield();
                        disposable0 = new Disposable(1).Using(c);
                        disposable1 = new Disposable(2).Using(c);
                        disposable2 = new Disposable(3).Using(c);
                    });

                "When using the disposables"
                    .x(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class StepsWithDisposablesAndTeardowns
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .x(c => new Disposable(1).Using(c))
                    .Teardown(() => typeof(ObjectDisposalFeature).SaveTestEvent("teardown2"));

                "And something else"
                    .x(c => new Disposable(3).Using(c))
                    .Teardown(() => typeof(ObjectDisposalFeature).SaveTestEvent("teardown4"));
            }
        }

        private class Disposable : IDisposable
        {
            private readonly int number;
            private bool isDisposed;

            public Disposable(int number)
            {
                this.number = number;
            }

            ~Disposable()
            {
                this.Dispose(false);
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
                    var @event = string.Concat("disposed", this.number.ToString(CultureInfo.InvariantCulture));
                    typeof(ObjectDisposalFeature).SaveTestEvent(@event);
                    this.isDisposed = true;
                }
            }
        }

        private sealed class BadDisposable : Disposable
        {
            public BadDisposable(int number)
                : base(number)
            {
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
