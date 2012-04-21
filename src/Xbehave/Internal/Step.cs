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
        private int millisecondsTimeout;

        public Step(string message, Func<IDisposable> action)
        {
            Require.NotNull(action, "action");

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.message = message;
            this.action = action;
        }

        public string Message
        {
            get { return this.message; }
        }

        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1500:VariableNamesShouldNotMatchFieldNames",
            MessageId = "millisecondsTimeout",
            Justification = "StyleCop enforces the 'this.' prefix when referencing an instance field.")]
        public IStep WithTimeout(int millisecondsTimeout)
        {
            this.millisecondsTimeout = millisecondsTimeout;
            return this;
        }

        public IDisposable Execute()
        {
            if (this.millisecondsTimeout > 0)
            {
                var result = this.action.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(this.millisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(this.millisecondsTimeout);
                }

                return this.action.EndInvoke(result);
            }

            return this.action();
        }
    }
}