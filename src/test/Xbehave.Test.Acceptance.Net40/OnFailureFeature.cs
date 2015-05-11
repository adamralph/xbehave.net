// <copyright file="OnFailureFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Sdk;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    public class OnFailureFeature : Feature
    {
        [Scenario]
        public void FailureBeforeForcedStep(Type feature, ITestResultMessage[] results)
        {
            ("Given a scenario with two empty steps, " +
                "a failing step with continuation, " +
                "two more empty steps, " +
                "a failing step and " +
                "another empty step")
                .f(() => feature = typeof(Steps));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be 7 results"
                .f(() => results.Length.Should().Be(7));

            "Then the first and second results are passes"
                .f(() => results.Take(2).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the third result is a failure"
                .f(() => results.Skip(2).Take(1).Should().ContainItemsAssignableTo<ITestFailed>());

            "And the fourth and fifth results are passes"
                .f(() => results.Skip(3).Take(2).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the sixth result is a failure"
                .f(() => results.Skip(5).Take(1).Should().ContainItemsAssignableTo<ITestFailed>());

            "And the seventh result is a skip"
                .f(() => results.Skip(6).Take(1).Should().ContainItemsAssignableTo<ITestSkipped>());
        }

        private static class Steps
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When something"
                    .f(() => { });

                "Then something which fails"
                    .f(() =>
                    {
                        throw new InvalidOperationException("oops");
                    })
                    .OnFailure(RemainingSteps.Run);

                "And something"
                    .f(() => { });

                "And something"
                    .f(() => { });

                "And something which fails"
                    .f(() =>
                    {
                        throw new InvalidOperationException("oops");
                    });

                "And something"
                    .f(() => { });
            }
        }
    }
}
#else
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    public class OnFailureFeature : Feature
    {
        [Scenario]
        public void FailureBeforeContinuationStep(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a failure before the continuation step"
                .f(() => feature = typeof(ScenarioWithFailureBeforeContinuationStep));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be four results"
                .f(() => results.Length.Should().Be(4));

            "Then the first result is a pass"
                .f(() => results.Take(1).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the second result is a failure"
                .f(() => results.Skip(1).Take(1).Should().ContainItemsAssignableTo<ITestFailed>());

            "And the last two results are failures"
                .f(() => results.Skip(2).Should().ContainItemsAssignableTo<ITestFailed>());
        }

        [Scenario]
        public void FailureDuringContinuationStep(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a failure during the continuation step"
                .f(() => feature = typeof(ScenarioWithFailureDuringContinuationStep));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be four results"
                .f(() => results.Length.Should().Be(4));

            "Then the first two results are passes"
                .f(() => results.Take(2).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the third result is a failure"
                .f(() => results.Skip(2).Take(1).Should().ContainItemsAssignableTo<ITestFailed>());

            "And the last result is a pass"
                .f(() => results.Skip(3).Should().ContainItemsAssignableTo<ITestPassed>());
        }

        [Scenario]
        public void FailureAfterContinuationStep(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a failure after the continuation step"
                .f(() => feature = typeof(ScenarioWithFailureAfterContinuationStep));

            "When I run the scenarios"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be five results"
                .f(() => results.Length.Should().Be(5));

            "Then the first three results are passes"
                .f(() => results.Take(3).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the fourth result is a failure"
                .f(() => results.Skip(3).Take(1).Should().ContainItemsAssignableTo<ITestFailed>());

            "And the last result is a pass"
                .f(() => results.Skip(4).Should().ContainItemsAssignableTo<ITestPassed>());
        }

        private static class ScenarioWithFailureBeforeContinuationStep
        {
            [Scenario]
            [ContinueOnFailureAfter(StepType.Then)]
            public static void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When something"
                    .f(() =>
                    {
                        throw new InvalidOperationException("oops");
                    });

                "Then something"
                    .f(() => { });

                "And something"
                    .f(() => { });
            }
        }

        private static class ScenarioWithFailureDuringContinuationStep
        {
            [Scenario]
            [ContinueOnFailureAfter(StepType.Then)]
            public static void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When something"
                    .f(() => { });

                "Then something"
                    .f(() =>
                    {
                        throw new InvalidOperationException("oops");
                    });

                "And something"
                    .f(() => { });
            }
        }

        private static class ScenarioWithFailureAfterContinuationStep
        {
            [Scenario]
            [ContinueOnFailureAfter(StepType.Then)]
            public static void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When something"
                    .f(() => { });

                "Then something"
                    .f(() => { });

                "And something"
                    .f(() =>
                    {
                        throw new InvalidOperationException("oops");
                    });

                "And something else"
                    .f(() => { });
            }
        }
    }
}
#endif
