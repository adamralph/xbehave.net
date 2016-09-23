// <copyright file="BeforeAfterTestFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class BeforeAfterTestFeature : Feature
    {
        [Background]
        public void Background()
        {
            "Given no events have occurred"
                .x(() => typeof(BeforeAfterTestFeature).ClearTestEvents());
        }

        [Scenario]
        public void BeforeAfterAttribute(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a before and after attribute"
                .x(() => feature = typeof(ScenarioWithBeforeAfterTestAttribute));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the attributes before and after methods are called before and after the scenario"
                .x(() => typeof(BeforeAfterTestFeature).GetTestEvents().Should().Equal(
                    "before1", "step1", "step2", "step3", "after1"));
        }

        [Scenario]
        public void ThrowsBefore(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a throw before attribute"
                .x(() => feature = typeof(ScenarioWithThrowBeforeAttribute));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there is a single test failure"
                .x(() => results.Should().ContainSingle(result => result is ITestFailed));
        }

        [Scenario]
        public void ThrowsAfter(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a throw after attribute"
                .x(() => feature = typeof(ScenarioWithThrowAfterAttribute));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there is a single test failure"
                .x(() => results.Should().ContainSingle(result => result is ITestFailed));
        }

        private static class ScenarioWithBeforeAfterTestAttribute
        {
            [BeforeAfter]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .x(() => typeof(BeforeAfterTestFeature).SaveTestEvent("step1"));

                "When"
                    .x(() => typeof(BeforeAfterTestFeature).SaveTestEvent("step2"));

                "Then"
                    .x(() => typeof(BeforeAfterTestFeature).SaveTestEvent("step3"));
            }
        }

        private static class ScenarioWithThrowBeforeAttribute
        {
            [ThrowBefore]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .x(() => { });

                "When"
                    .x(() => { });

                "Then"
                    .x(() => { });
            }
        }

        private static class ScenarioWithThrowAfterAttribute
        {
            [ThrowAfter]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .x(() => { });

                "When"
                    .x(() => { });

                "Then"
                    .x(() => { });
            }
        }

        private sealed class BeforeAfter : BeforeAfterTestAttribute
        {
            private static int beforeCount;
            private static int afterCount;

            public override void Before(System.Reflection.MethodInfo methodUnderTest)
            {
                beforeCount++;
                typeof(BeforeAfterTestFeature)
                    .SaveTestEvent("before" + beforeCount.ToString(CultureInfo.InvariantCulture));
            }

            public override void After(System.Reflection.MethodInfo methodUnderTest)
            {
                afterCount++;
                typeof(BeforeAfterTestFeature)
                    .SaveTestEvent("after" + afterCount.ToString(CultureInfo.InvariantCulture));
            }
        }

        private sealed class ThrowBefore : BeforeAfterTestAttribute
        {
            public override void Before(System.Reflection.MethodInfo methodUnderTest)
            {
                throw new InvalidOperationException();
            }
        }

        private sealed class ThrowAfter : BeforeAfterTestAttribute
        {
            public override void After(System.Reflection.MethodInfo methodUnderTest)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
