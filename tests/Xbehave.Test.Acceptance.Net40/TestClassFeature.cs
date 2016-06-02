// <copyright file="TestClassFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Globalization;
    using System.Threading;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;

    public class TestClassFeature : Feature
    {
        [Background]
        public void Background()
        {
            "Given no events have occurred"
                .f(() => typeof(TestClassFeature).ClearTestEvents());
        }

        [Scenario]
        public void SingleScenario(Type feature)
        {
            "Given an instance scenario with three steps in a disposable type"
                .f(() => feature = typeof(InstanceScenarioWithThreeStepsInADisposableType));

            "When I run the scenario"
                .f(() => this.Run(feature));

            "Then an instance of the type is created and disposed once either side of the step execution"
                .f(() => typeof(TestClassFeature).GetTestEvents()
                    .Should().Equal("created1", "step1", "step2", "step3", "disposed1.1"));
        }

        private class InstanceScenarioWithThreeStepsInADisposableType : IDisposable
        {
            private static int instanceCount;
            private readonly int instanceNumber;
            private int disposalCount;

            public InstanceScenarioWithThreeStepsInADisposableType()
            {
                this.instanceNumber = Interlocked.Increment(ref instanceCount);
                typeof(TestClassFeature).SaveTestEvent(
                    string.Concat("created", this.instanceNumber.ToString(CultureInfo.InvariantCulture)));
            }

            [Scenario]
            public void Scenario()
            {
                "Given"
                    .f(() => typeof(TestClassFeature).SaveTestEvent("step1"));

                "When"
                    .f(() => typeof(TestClassFeature).SaveTestEvent("step2"));

                "Then"
                    .f(() => typeof(TestClassFeature).SaveTestEvent("step3"));
            }

            public void Dispose()
            {
                var @event = string.Concat(
                    "disposed", this.instanceNumber.ToString(CultureInfo.InvariantCulture), ".", ++this.disposalCount);

                typeof(TestClassFeature).SaveTestEvent(@event);
            }
        }
    }
}
#endif
