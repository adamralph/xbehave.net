// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class Step
    {
        private readonly string name;
        private readonly Action body;
        private readonly List<Action> teardowns = new List<Action>();

        public Step(string name, Action body)
        {
            LiteGuard.Guard.AgainstNullArgument("body", body);

            this.name = name;
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

        public void Execute()
        {
            try
            {
                if (this.MillisecondsTimeout > 0)
                {
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
                this.teardowns.ForEach(CurrentScenario.AddTeardown);
            }
        }
    }
}