namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunner : XunitTestRunner
    {
        private readonly IScenario scenario;
        private readonly IMessageBus messageBus;
        private readonly Type scenarioClass;
        private readonly object[] constructorArguments;
        private readonly MethodInfo scenarioMethod;
        private readonly object[] scenarioMethodArguments;
        private readonly string skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes;
        private readonly ExceptionAggregator parentAggregator;
        private readonly CancellationTokenSource cancellationTokenSource;

        public ScenarioRunner(ITest test,
                               IMessageBus messageBus,
                               Type testClass,
                               object[] constructorArguments,
                               MethodInfo testMethod,
                               object[] testMethodArguments,    
                               string skipReason,
                               IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
                               ExceptionAggregator aggregator,
                               CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        private async Task<RunSummary> InvokeScenarioAsync(ExceptionAggregator aggregator) =>
            await new ScenarioInvoker(
                    this.scenario,
                    this.messageBus,
                    this.scenarioClass,
                    this.constructorArguments,
                    this.scenarioMethod,
                    this.scenarioMethodArguments,
                    this.beforeAfterScenarioAttributes,
                    aggregator,
                    this.cancellationTokenSource)
                .RunAsync();
    }
}
