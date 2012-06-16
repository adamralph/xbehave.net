// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Infra;

    internal class Step
    {
        private readonly string name;
        private readonly Action body;
        private readonly Action teardown;

        public Step(string stepType, string text, Action body, Action teardown, bool inIsolation, string skipReason)
        {
            Guard.AgainstNullArgument("stepType", stepType);
            Guard.AgainstNullArgument("text", text);
            Guard.AgainstNullArgument("body", body);

            this.name = (stepType.CompressWhitespace() + " ").MergeOrdinalIgnoreCase(text.CompressWhitespace());
            this.body = body;
            this.teardown = teardown;
            this.InIsolation = inIsolation;
            this.SkipReason = skipReason;
        }

        public string Name
        {
            get { return this.name; }
        }

        public string SkipReason { get; set; }

        public bool InIsolation { get; set; }

        public int MillisecondsTimeout { get; set; }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed after step execution.")]
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

                this.body();
            }
            finally
            {
                if (this.teardown != null)
                {
                    CurrentScenario.AddDisposable(new Disposable(this.teardown));
                }
            }
        }
    }
}