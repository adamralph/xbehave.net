namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioTestCase : XunitTestCase
    {
        private readonly IScenario scenario;
        private readonly Type scenarioClass;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes;
        private readonly MethodInfo scenarioMethod;

        public ScenarioTestCase(
            IScenario scenario,
            Type scenarioClass,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes,
            MethodInfo scenarioMethod,
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            object[] testMethodArguments)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            this.scenario = scenario;
            this.scenarioClass = scenarioClass;
            this.beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
            this.scenarioMethod = scenarioMethod;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", true)]
        public ScenarioTestCase()
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource) =>
                await new ScenarioRunner(
                    this.scenario,
                    messageBus,
                    this.scenarioClass,
                    constructorArguments,
                    this.scenarioMethod,
                    this.TestMethodArguments,
                    this.SkipReason,
                    this.beforeAfterScenarioAttributes,
                    aggregator,
                    cancellationTokenSource)
                    .RunAsync();
    }
}
