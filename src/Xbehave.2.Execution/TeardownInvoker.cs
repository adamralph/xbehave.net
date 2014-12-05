// <copyright file="TeardownInvoker.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class TeardownInvoker : XbehaveDelegateInvoker
    {
        private readonly Action[] teardowns;

        public TeardownInvoker(
            Action[] teardowns,
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
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
                beforeAfterAttributes,
                aggregator,
                cancellationTokenSource)
        {
            Guard.AgainstNullArgument("teardowns", teardowns);

            this.teardowns = teardowns.ToArray();
        }

        [SuppressMessage(
            "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Emulating try finally.")]
        protected override Task InvokeDelegatesAsync()
        {
            Exception exception = null;
            foreach (var teardown in this.teardowns.Reverse())
            {
                try
                {
                    teardown();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            if (exception != null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            return Task.FromResult(0);
        }
    }
}
