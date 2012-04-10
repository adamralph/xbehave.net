// <copyright file="StepBase.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    internal abstract class StepBase<T> : IStep
    {
        private readonly string message;
        private readonly T action;

        protected StepBase(string message, T action)
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
    }
}