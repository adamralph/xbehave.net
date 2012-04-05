// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Legacy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    internal class Step<T> : IStep
    {
        private readonly string message;
        private readonly T action;
        private int millisecondsTimeout;

        public Step(string message, T action)
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

        public T Action
        {
            get { return this.action; }
        }

        public int MillisecondsTimeout
        {
            get { return this.millisecondsTimeout; }
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
    }
}