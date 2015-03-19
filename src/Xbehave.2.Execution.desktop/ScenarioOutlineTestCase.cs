// <copyright file="ScenarioOutlineTestCase.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    [Serializable]
    public class ScenarioOutlineTestCase : XunitTestCase
    {
        public ScenarioOutlineTestCase(
            IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
            : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, null)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", true)]
        public ScenarioOutlineTestCase()
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return await new ScenarioOutlineTestCaseRunner(
                    diagnosticMessageSink,
                    this,
                    this.DisplayName,
                    this.SkipReason,
                    constructorArguments,
                    messageBus,
                    aggregator,
                    cancellationTokenSource)
                .RunAsync();
        }
    }
}
