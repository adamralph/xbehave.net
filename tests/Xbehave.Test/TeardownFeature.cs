namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave.Test.Infrastructure;
    using Xunit.Abstractions;

    // In order to release allocated resources
    // As a developer
    // I want to execute teardowns after a scenario has run
    public class TeardownFeature : Feature
    {
        [Background]
        public void Background() =>
            "Given no events have occurred"
                .x(() => typeof(TeardownFeature).ClearTestEvents());

        [Scenario]
        public void ManyTeardownsInASingleStep(Type feature, ITestResultMessage[] results)
        {
            "Given a step with many teardowns"
                .x(() => feature = typeof(StepWithManyTeardowns));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one resilt"
                .x(() => results.Length.Should().Be(1));

            "And there should be no failures"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());

            "Ann the teardowns should be executed in reverse order after the step"
                .x(() => typeof(TeardownFeature).GetTestEvents()
                    .Should().Equal("step1", "teardown3", "teardown2", "teardown1"));
        }

        [Scenario]
        public void TeardownsWhichThrowExceptionsWhenExecuted(Type feature, ITestResultMessage[] results)
        {
            "Given a step with three teardowns which throw exceptions when executed"
                .x(() => feature = typeof(StepWithThreeBadTeardowns));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be two results"
                .x(() => results.Length.Should().Be(2));

            "And the first result should be a pass"
                .x(() => results[0].Should().BeAssignableTo<ITestPassed>());

            "And the second result should be a failure"
                .x(() => results[1].Should().BeAssignableTo<ITestFailed>());

            "And the name of the teardown should end in '(Teardown)'"
                .x(() => results[1].Test.DisplayName.Should().EndWith("(Teardown)"));

            "And the teardowns should be executed in reverse order after the step"
                .x(() => typeof(TeardownFeature).GetTestEvents()
                    .Should().Equal("step1", "teardown3", "teardown2", "teardown1"));
        }

        [Scenario]
        public void ManyTeardownsInManySteps(Type feature, ITestResultMessage[] results)
        {
            "Given two steps with three teardowns each"
                .x(() => feature = typeof(TwoStepsWithThreeTeardownsEach));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be two results"
                .x(() => results.Length.Should().Be(2));

            "And there should be no failures"
                .x(() => results.Should().ContainItemsAssignableTo<ITestPassed>());

            "And the teardowns should be executed in reverse order after the steps"
                .x(() => typeof(TeardownFeature).GetTestEvents().Should().Equal(
                    "step1", "step2", "teardown6", "teardown5", "teardown4", "teardown3", "teardown2", "teardown1"));
        }

        [Scenario]
        public void FailingSteps(Type feature, ITestResultMessage[] results)
        {
            "Given two steps with teardowns and a failing step"
                .x(() => feature = typeof(TwoStepsWithTeardownsAndAFailingStep));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one failure"
                .x(() => results.OfType<ITestFailed>().Count().Should().Be(1));

            "And the teardowns should be executed after each step"
                .x(() => typeof(TeardownFeature).GetTestEvents()
                    .Should().Equal("step1", "step2", "step3", "teardown2", "teardown1"));
        }

        [Scenario]
        public void FailureToCompleteAStep(Type feature, ITestResultMessage[] results)
        {
            "Given a failing step with three teardowns"
                .x(() => feature = typeof(FailingStepWithThreeTeardowns));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one failure"
                .x(() => results.OfType<ITestFailed>().Count().Should().Be(1));

            "And the teardowns should be executed in reverse order after the step"
                .x(() => typeof(TeardownFeature).GetTestEvents()
                    .Should().Equal("step1", "teardown3", "teardown2", "teardown1"));
        }

        [Scenario]
        public void NullTeardown() =>
            "Given a null body"
                .x(() => { })
                .Teardown((Action)null);

        [Scenario]
        public void AsyncTeardowns(Type feature, ITestResultMessage[] results)
        {
            "Given a step with an async teardown which throws"
                .x(() => feature = typeof(StepWithAnAsyncTeardownWhichThrows));

            "When running the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one failure"
                .x(() => results.OfType<ITestFailed>().Count().Should().Be(1));
        }

        private static class StepWithManyTeardowns
        {
            [Scenario]
            public static void Scenario() =>
                "Given something"
                    .x(() => typeof(TeardownFeature).SaveTestEvent("step1"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown1"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown2"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown3"));
        }

        private static class StepWithThreeBadTeardowns
        {
            [Scenario]
            public static void Scenario() =>
                "Given something"
                    .x(() => typeof(TeardownFeature).SaveTestEvent("step1"))
                    .Teardown(() =>
                    {
                        typeof(TeardownFeature).SaveTestEvent("teardown1");
                        throw new InvalidOperationException();
                    })
                    .Teardown(() =>
                    {
                        typeof(TeardownFeature).SaveTestEvent("teardown2");
                        throw new InvalidOperationException();
                    })
                    .Teardown(() =>
                    {
                        typeof(TeardownFeature).SaveTestEvent("teardown3");
                        throw new InvalidOperationException();
                    });
        }

        private static class TwoStepsWithThreeTeardownsEach
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .x(() => typeof(TeardownFeature).SaveTestEvent("step1"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown1"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown2"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown3"));

                "And something else"
                    .x(() => typeof(TeardownFeature).SaveTestEvent("step2"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown4"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown5"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown6"));
            }
        }

        private static class TwoStepsWithTeardownsAndAFailingStep
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .x(() => typeof(TeardownFeature).SaveTestEvent("step1"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown1"));

                "When something"
                    .x(() => typeof(TeardownFeature).SaveTestEvent("step2"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown2"));

                "Then something happens"
                    .x(() =>
                    {
                        typeof(TeardownFeature).SaveTestEvent("step3");
                        1.Should().Be(0);
                    });
            }
        }

        private static class FailingStepWithThreeTeardowns
        {
            [Scenario]
            public static void Scenario() =>
                "Given something"
                    .x(() =>
                    {
                        typeof(TeardownFeature).SaveTestEvent("step1");
                        throw new InvalidOperationException();
                    })
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown1"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown2"))
                    .Teardown(() => typeof(TeardownFeature).SaveTestEvent("teardown3"));
        }

        private static class StepWithAnAsyncTeardownWhichThrows
        {
            [Scenario]
            public static void Scenario() =>
                "Given something"
                    .x(() => typeof(TeardownFeature).SaveTestEvent("step1"))
                    .Teardown(async () =>
                    {
                        await Task.Yield();
                        throw new Exception();
                    });
        }
    }
}
