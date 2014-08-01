// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class Step : XunitTestCase
    {
        private readonly string name;
        private readonly Func<Task> body;

        public Step(ITestMethod testMethod, string name, Func<Task> body)
            : base(testMethod)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name consists of only whitespace.", "name");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name consists of only whitespace.", "name");
            }

            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            this.name = name;
            this.body = body;
            this.DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.DisplayName, name);
        }

        public string Name
        {
            get { return this.name; }
        }

        public Func<Task> Body
        {
            get { return this.body; }
        }

        public override async Task<RunSummary> RunAsync(
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return await new StepRunner(this, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }
    }
}
