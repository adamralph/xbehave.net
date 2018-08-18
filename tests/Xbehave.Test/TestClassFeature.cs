namespace Xbehave.Test
{
    using System;
    using System.Globalization;
    using System.Threading;
    using FluentAssertions;
    using Xbehave.Test.Infrastructure;

    public class TestClassFeature : Feature
    {
        [Background]
        public void Background() =>
            "Given no events have occurred"
                .x(() => typeof(TestClassFeature).ClearTestEvents());

        [Scenario]
        public void SingleScenario(Type feature)
        {
            "Given an instance scenario with three steps in a disposable type"
                .x(() => feature = typeof(InstanceScenarioWithThreeStepsInADisposableType));

            "When I run the scenario"
                .x(() => this.Run(feature));

            "Then an instance of the type is created and disposed once either side of the step execution"
                .x(() => typeof(TestClassFeature).GetTestEvents()
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
                    .x(() => typeof(TestClassFeature).SaveTestEvent("step1"));

                "When"
                    .x(() => typeof(TestClassFeature).SaveTestEvent("step2"));

                "Then"
                    .x(() => typeof(TestClassFeature).SaveTestEvent("step3"));
            }

            public void Dispose()
            {
                var @event = string.Concat(
                    "disposed",
                    this.instanceNumber.ToString(CultureInfo.InvariantCulture),
                    ".",
                    (++this.disposalCount).ToString(CultureInfo.InvariantCulture));

                typeof(TestClassFeature).SaveTestEvent(@event);
            }
        }
    }
}
