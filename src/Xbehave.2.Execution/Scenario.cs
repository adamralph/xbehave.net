// <copyright file="Scenario.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    [Serializable]
    public class Scenario : XunitTestCase
    {
        public Scenario(ITestMethod method)
            : base(method)
        {
        }

        protected Scenario(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            var timer = new ExecutionTimer();
            var summary = new RunSummary();
            try
            {
                await timer.AggregateAsync(async () =>
                {
                    var type = Reflector.GetType(
                        this.TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name,
                        this.TestMethod.TestClass.Class.Name);

                    var method = type.GetMethod(this.TestMethod.Method.Name, this.TestMethod.Method.GetBindingFlags());
                    var obj = method.IsStatic ? null : Activator.CreateInstance(type, constructorArguments);
                    var result = method.Invoke(obj, new object[0]);
                    var task = result as Task;
                    if (task != null)
                    {
                        await task;
                    }
                });

                var stepFailed = false;
                StepTestCase failedStep = null;
                var interceptingBus = new DelegatingMessageBus(
                    messageBus,
                    message =>
                    {
                        if (message is ITestFailed)
                        {
                            stepFailed = true;
                        }
                    });

                foreach (var testCase in CurrentScenario.ExtractSteps()
                    .Select(step => new StepTestCase(this.TestMethod, step)))
                {
                    if (failedStep != null)
                    {
                        var message = string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to execute preceding step \"{0}\".",
                            failedStep.Step.Name);

                        var failingTestCase = new LambdaTestCase(
                            this.TestMethod,
                            () =>
                            {
                                throw new InvalidOperationException(message);
                            });

                        await failingTestCase
                            .RunAsync(messageBus, constructorArguments, aggregator, cancellationTokenSource);

                        continue;
                    }

                    summary.Aggregate(
                        await testCase.RunAsync(
                            interceptingBus, constructorArguments, aggregator, cancellationTokenSource));

                    if (stepFailed)
                    {
                        failedStep = testCase;
                    }
                }
            }
            catch (Exception ex)
            {
                summary.Failed++;
                if (!messageBus.QueueMessage(new TestFailed(this, DisplayName, timer.Total, null, ex)))
                {
                    cancellationTokenSource.Cancel();
                }
            }

            return summary;
        }
    }
}
