// <copyright file="DisposalCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Sdk.Infra;
    using Xunit.Sdk;

    internal class DisposalCommand : CommandBase
    {
        private readonly IDisposer disposer = new Disposer();
        private readonly IEnumerable<IDisposable> disposables;

        public DisposalCommand(MethodCall call, int? contextOrdinal, int ordinal, IEnumerable<IDisposable> disposables)
            : base(call, contextOrdinal, ordinal, "Disposal")
        {
            this.disposables = disposables;
        }

        public override MethodResult Execute(object testClass)
        {
            this.disposer.Dispose(this.disposables);
            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}