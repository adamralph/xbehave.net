// <copyright file="ScenarioOutline.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    [Serializable]
    public class ScenarioOutline : XunitTestCase
    {
        public ScenarioOutline(ITestMethod method)
            : base(method)
        {
        }

        protected ScenarioOutline(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return await new ScenarioOutlineRunner(
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
