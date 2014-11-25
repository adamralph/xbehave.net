// <copyright file="StepRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Execution.Shims;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class StepRunner : TestRunner<IXunitTestCase>
    {
        private readonly string stepName;
        private readonly Func<object> stepBody;

        public StepRunner(
            string stepName,
            Func<object> stepBody,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                test,
                messageBus,
                testClass,
                constructorArguments,
                testMethod,
                testMethodArguments,
                skipReason,
                aggregator,
                cancellationTokenSource)
        {
            Guard.AgainstNullArgument("stepBody", stepBody);

            this.stepName = stepName;
            this.stepBody = stepBody;
        }

        public string StepName
        {
            get { return this.stepName; }
        }

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var output = string.Empty;
            var testOutputHelper = ConstructorArguments.OfType<TestOutputHelper>().FirstOrDefault();
            if (testOutputHelper != null)
            {
                testOutputHelper.Initialize(this.MessageBus, this.Test);
            }

            var timer = new ExecutionTimer();
            var oldSyncContext = SynchronizationContext.Current;

            try
            {
                var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                SetSynchronizationContext(asyncSyncContext);

                await aggregator.RunAsync(
                    () => timer.AggregateAsync(
                        async () =>
                        {
                            var result = this.stepBody();
                            var task = result as Task;
                            if (task != null)
                            {
                                await task;
                            }
                            else
                            {
                                var ex = await asyncSyncContext.WaitForCompletionAsync();
                                if (ex != null)
                                {
                                    aggregator.Add(ex);
                                }
                            }
                        }));
            }
            finally
            {
                SetSynchronizationContext(oldSyncContext);
            }

            var executionTime = timer.Total;

            if (testOutputHelper != null)
            {
                output = testOutputHelper.Output;
                testOutputHelper.Uninitialize();
            }

            return Tuple.Create(executionTime, output);
        }

        [SecuritySafeCritical]
        private static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
