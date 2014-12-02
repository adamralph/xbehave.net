// <copyright file="BeforeAfterTestFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if !V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
#if !V2
    using Xunit;
#else
    using Xunit.Sdk;
#endif

    public static class BeforeAfterTestFeature
    {
        [Scenario]
        public static void BeforeAfterAttribute(Type feature, Result[] results)
        {
            "Given a scenario with a before and after attribute"
                .f(() => feature = typeof(ScenarioWithBeforeAfterTestAttribute));

            "When I run the scenario"
                .f(() => results = feature.RunScenarios());

            "Then the attributes before and after methods are called before and after each step"
                .f(() => results.Should().ContainItemsAssignableTo<Pass>());
        }

        private static class ScenarioWithBeforeAfterTestAttribute
        {
            [BeforeAfter]
            [Scenario]
            public static void Scenario()
            {
                "Given"
                    .f(() =>
                    {
                        BeforeAfter.BeforeCount.Should().Be(1, "the before method should have called once");
                        BeforeAfter.AfterCount.Should().Be(0, "the after method should not have been called");
                    });

                "When"
                    .f(() =>
                    {
                        BeforeAfter.BeforeCount.Should().Be(2, "the before method should have called twice");
                        BeforeAfter.AfterCount.Should().Be(1, "the after method should have called once");
                    });

                "Then"
                    .f(() =>
                    {
                        BeforeAfter.BeforeCount.Should().Be(3, "the before method should have called thrice");
                        BeforeAfter.AfterCount.Should().Be(2, "the after method should have called twice");
                    });
            }
        }

        private class BeforeAfter : BeforeAfterTestAttribute
        {
            private static int beforeCount;
            private static int afterCount;

            public static int BeforeCount
            {
                get { return beforeCount; }
            }

            public static int AfterCount
            {
                get { return afterCount; }
            }

            public override void Before(System.Reflection.MethodInfo methodUnderTest)
            {
                beforeCount++;
                afterCount.Should().Be(beforeCount - 1);
            }

            public override void After(System.Reflection.MethodInfo methodUnderTest)
            {
                afterCount++;
                afterCount.Should().Be(beforeCount);
            }
        }
    }
}
#endif
