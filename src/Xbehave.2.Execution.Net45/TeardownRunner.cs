// <copyright file="TeardownRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class TeardownRunner : XbehaveTestRunner
    {
        private readonly Action[] teardowns;

        public TeardownRunner(
            Action[] teardowns,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
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
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            Guard.AgainstNullArgument("teardowns", teardowns);

            this.teardowns = teardowns.ToArray();
        }

        protected async override Task<decimal> InvokeDelegatesAsync(ExceptionAggregator aggregator)
        {
            return await new TeardownInvoker(
                this.teardowns,
                this.Test,
                this.MessageBus,
                this.TestClass,
                this.ConstructorArguments,
                this.TestMethod,
                this.TestMethodArguments,
                this.BeforeAfterAttributes,
                aggregator,
                this.CancellationTokenSource).RunAsync();
        }
    }
}
