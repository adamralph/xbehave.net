// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Sdk.Infrastructure;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class Step
    {
        private readonly string name;
        private readonly Action body;
        private readonly List<Action> teardowns = new List<Action>();

        public Step(string stepType, string description, Action body)
        {
            Guard.AgainstNullArgument("stepType", stepType);
            Guard.AgainstNullArgument("description", description);
            Guard.AgainstNullArgument("body", body);

            if (stepType.Length == 0)
            {
                // TODO: cover with acceptance test
                throw new ArgumentException("stepType is empty.", "stepType");
            }

            if (description.Length == 0)
            {
                // TODO: cover with acceptance test
                throw new ArgumentException("description is empty.", "description");
            }

            this.name = (stepType.CompressWhiteSpace() + " ").MergeOrdinalIgnoreCase(description.CompressWhiteSpace());
            this.body = body;
        }

        public virtual string Name
        {
            get { return this.name; }
        }

        public string SkipReason { get; set; }

        public bool InIsolation { get; set; }

        public int MillisecondsTimeout { get; set; }

        public void AddTeardown(Action teardown)
        {
            if (teardown != null)
            {
                this.teardowns.Add(teardown);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed after step execution.")]
        public void Execute()
        {
            try
            {
                if (this.MillisecondsTimeout > 0)
                {
                    // TODO: cover with acceptance test
                    var result = this.body.BeginInvoke(null, null);

                    // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                    if (!result.AsyncWaitHandle.WaitOne(this.MillisecondsTimeout, false))
                    {
                        throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                    }

                    this.body.EndInvoke(result);
                }
                else
                {
                    this.body();
                }
            }
            finally
            {
                foreach (var teardown in this.teardowns)
                {
                    CurrentScenario.AddDisposable(new Disposable(teardown));
                }
            }
        }
    }
}