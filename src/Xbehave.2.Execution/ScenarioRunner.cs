// <copyright file="ScenarioRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioRunner : XunitTestCaseRunner
    {
        public ScenarioRunner(
            IXunitTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                testCase,
                displayName,
                skipReason,
                constructorArguments,
                testMethodArguments,
                messageBus,
                aggregator,
                cancellationTokenSource)
        {
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var timer = new ExecutionTimer();
            var summary = new RunSummary();
            try
            {
                await timer.AggregateAsync(async () =>
                {
                    var type = Reflector.GetType(
                        this.TestCase.TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name,
                        this.TestCase.TestMethod.TestClass.Class.Name);

                    var method = type.GetMethod(
                        this.TestCase.TestMethod.Method.Name, this.TestCase.TestMethod.Method.GetBindingFlags());

                    var obj = method.IsStatic ? null : Activator.CreateInstance(type, this.ConstructorArguments);
                    var result = method.Invoke(obj, this.TestMethodArguments);
                    var task = result as Task;
                    if (task != null)
                    {
                        await task;
                    }
                });

                var stepFailed = false;
                string failedStepName = null;
                var interceptingBus = new DelegatingMessageBus(
                    this.MessageBus,
                    message =>
                    {
                        if (message is ITestFailed)
                        {
                            stepFailed = true;
                        }
                    });

                foreach (var stepDefinition in CurrentScenario.ExtractStepDefinitions()
                    .Select(definition => new Step(this.TestCase.TestMethod, definition.Name, definition.Body)))
                {
                    if (failedStepName != null)
                    {
                        var message = string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed to execute preceding step \"{0}\".",
                            failedStepName);

                        var failFast = new LambdaTestCase(
                            this.TestCase.TestMethod,
                            () =>
                            {
                                throw new InvalidOperationException(message);
                            });

                        await failFast.RunAsync(
                            this.MessageBus, this.ConstructorArguments, this.Aggregator, this.CancellationTokenSource);

                        continue;
                    }

                    summary.Aggregate(
                        await stepDefinition.RunAsync(
                            interceptingBus, this.ConstructorArguments, this.Aggregator, this.CancellationTokenSource));

                    if (stepFailed)
                    {
                        failedStepName = stepDefinition.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                summary.Failed++;
                if (!this.MessageBus.QueueMessage(
                    new TestFailed(this.TestCase, this.DisplayName, timer.Total, null, ex)))
                {
                    this.CancellationTokenSource.Cancel();
                }
            }

            return summary;
        }
    }
}
