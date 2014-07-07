// <copyright file="Scenario.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

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
            var type = Reflector.GetType(
                this.TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name,
                this.TestMethod.TestClass.Class.Name);

            var method = type.GetMethod(this.TestMethod.Method.Name, this.TestMethod.Method.GetBindingFlags());
            method.Invoke(method.IsStatic ? null : Activator.CreateInstance(type), new object[0]);

            var summary = new RunSummary();
            foreach (var testCase in CurrentScenario.ExtractSteps()
                .Select(step => new StepTestCase(this.TestMethod, step)))
            {
                summary.Aggregate(
                    await testCase.RunAsync(messageBus, constructorArguments, aggregator, cancellationTokenSource));
            }

            return summary;
        }
    }
}
