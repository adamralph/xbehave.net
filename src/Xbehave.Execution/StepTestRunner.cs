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

    public class StepTestRunner : XunitTestRunner
    {
        private readonly IStepContext stepContext;
        private readonly Func<IStepContext, Task> body;

        public StepTestRunner(
            IStepContext stepContext,
            Func<IStepContext, Task> body,
            ITest test,
            IMessageBus messageBus,
            Type scenarioClass,
            object[] constructorArguments,
            MethodInfo scenarioMethod,
            object[] scenarioMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(
                test,
                messageBus,
                scenarioClass,
                constructorArguments,
                scenarioMethod,
                scenarioMethodArguments,
                skipReason,
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            this.stepContext = stepContext;
            this.body = body;
        }

        protected override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator) =>
            new StepInvoker(this.stepContext, this.body, aggregator, this.CancellationTokenSource).RunAsync();
    }
}
