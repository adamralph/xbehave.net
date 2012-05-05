// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using Xbehave.Infra;

    internal class Step
    {
        private readonly string message;
        private readonly Func<IDisposable> execute;
        private readonly bool inIsolation;
        private readonly string skipReason;

        public Step(string message, Func<IDisposable> execute)
        {
            Require.NotNull(execute, "execute");

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.message = message;
            this.execute = execute;
        }

        public Step(string message, Func<IDisposable> execute, bool inIsolation)
            : this(message, execute)
        {
            this.inIsolation = inIsolation;
        }

        public Step(string message, Func<IDisposable> execute, string skipReason)
            : this(message, execute)
        {
            this.skipReason = skipReason;
        }

        public string Message
        {
            get { return this.message; }
        }

        public bool InIsolation
        {
            get { return this.inIsolation; }
        }

        public int MillisecondsTimeout { get; set; }

        public IDisposable Execute()
        {
            if (this.MillisecondsTimeout > 0)
            {
                var result = this.execute.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(this.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                }

                return this.execute.EndInvoke(result);
            }

            return this.execute();
        }
    }
}