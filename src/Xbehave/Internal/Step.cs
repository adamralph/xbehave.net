// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    internal class Step : IStep
    {
        private readonly string message;
        private readonly Func<IDisposable> action;

        public Step(string message, Func<IDisposable> action)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.message = message;
            this.action = action;
        }

        public string Message
        {
            get { return this.message; }
        }

        public Func<IDisposable> Action
        {
            get { return this.action; }
        }

        public int MillisecondsTimeout { get; private set; }

        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1500:VariableNamesShouldNotMatchFieldNames",
            MessageId = "millisecondsTimeout",
            Justification = "StyleCop enforces the 'this.' prefix when referencing an instance field.")]
        public IStep WithTimeout(int millisecondsTimeout)
        {
            this.MillisecondsTimeout = millisecondsTimeout;
            return this;
        }

        public IDisposable Execute()
        {
            if (this.MillisecondsTimeout > 0)
            {
                var result = this.Action.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(this.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                }

                return this.Action.EndInvoke(result);
            }

            return this.Action();
        }
    }
}