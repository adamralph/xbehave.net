// <copyright file="ScenarioOutline.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
#if WPA81 || WINDOWS_PHONE
    using Xbehave.Execution.Shims;
    using Xunit;
#endif
    using Xunit.Abstractions;
    using Xunit.Sdk;

    [Serializable]
    public class ScenarioOutline : XunitTestCase
    {
        public ScenarioOutline(
            IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
            : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, null)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", true)]
        public ScenarioOutline()
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return await new ScenarioOutlineRunner(
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
