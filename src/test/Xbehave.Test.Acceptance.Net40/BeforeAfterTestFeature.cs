// <copyright file="BeforeAfterTestFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if NET40 || NET45
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
#if !V2
    using Xunit;
    using Xunit.Abstractions;
#else
    using Xunit.Abstractions;
    using Xunit.Sdk;
#endif

    public class BeforeAfterTestFeature : Feature
    {
        [Background]
        public void Background()
        {
            "Given no events have occurred"
                .f(() => typeof(BeforeAfterTestFeature).ClearTestEvents());
        }

        [Scenario]
        public void BeforeAfterAttribute(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a before and after attribute"
                .f(() => feature = typeof(ScenarioWithBeforeAfterTestAttribute));

            "When I run the scenario"
                .f(() => results = this.Run<ITestResultMessage>(feature));

#if !V2
            "Then the attributes before and after methods are called before and after each step"
                .f(() => typeof(BeforeAfterTestFeature).GetTestEvents().Should().Equal(
                    "before1", "step1", "after1", "before2", "step2", "after2", "before3", "step3", "after3"));
#else
            "Then the attributes before and after methods are called before and after the scenario"
                .f(() => typeof(BeforeAfterTestFeature).GetTestEvents().Should().Equal(
                    "before1", "step1", "step2", "step3", "after1"));
#endif
        }

        private static class ScenarioWithBeforeAfterTestAttribute
        {
            [BeforeAfter]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .f(() => typeof(BeforeAfterTestFeature).SaveTestEvent("step1"));

                "When"
                    .f(() => typeof(BeforeAfterTestFeature).SaveTestEvent("step2"));

                "Then"
                    .f(() => typeof(BeforeAfterTestFeature).SaveTestEvent("step3"));
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
    }
}
#endif
