// <copyright file="StepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Infra;

    internal partial class StepDefinition : IStepDefinition
    {
        public IStepDefinition When(string message, Func<IDisposable> body)
        {
            Guard.AgainstNullArgument("body", body);
            IDisposable disposable = null;
            return message.When(() => disposable = body(), () => new Disposable(new[] { disposable }).Dispose());
        }

        public IStepDefinition When(string message, Func<IEnumerable<IDisposable>> body)
        {
            Guard.AgainstNullArgument("body", body);
            IEnumerable<IDisposable> disposables = null;
            return message.When(() => disposables = body(), () => new Disposable(disposables).Dispose());
        }

        public IStepDefinition And(string message, Func<IDisposable> body)
        {
            Guard.AgainstNullArgument("body", body);
            IDisposable disposable = null;
            return message.And(() => disposable = body(), () => new Disposable(new[] { disposable }).Dispose());
        }

        public IStepDefinition And(string message, Func<IEnumerable<IDisposable>> body)
        {
            Guard.AgainstNullArgument("body", body);
            IEnumerable<IDisposable> disposables = null;
            return message.And(() => disposables = body(), () => new Disposable(disposables).Dispose());
        }
    }
}
