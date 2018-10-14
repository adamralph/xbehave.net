// https://github.com/xunit/xunit/blob/2.4/src/xunit.execution/Sdk/Frameworks/XunitTheoryTestCase.cs
namespace Xbehave.Execution
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioOutlineTestCase : XunitTestCase
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public ScenarioOutlineTestCase() { }

        public ScenarioOutlineTestCase(IMessageSink diagnosticMessageSink,
                                       TestMethodDisplay defaultMethodDisplay,
                                       TestMethodDisplayOptions defaultMethodDisplayOptions,
                                       ITestMethod testMethod)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod) { }

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                                        IMessageBus messageBus,
                                                        object[] constructorArguments,
                                                        ExceptionAggregator aggregator,
                                                        CancellationTokenSource cancellationTokenSource)
            => await new ScenarioOutlineTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource).RunAsync();
    }
}
