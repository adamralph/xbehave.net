// <copyright file="DisposalCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Sdk.Infrastructure;
    using Xunit.Sdk;

    public class DisposalCommand : CommandBase
    {
        private readonly IEnumerable<IDisposable> disposables;

        public DisposalCommand(ScenarioDefinition call, int contextOrdinal, int ordinal, IEnumerable<IDisposable> disposables)
            : base(call, contextOrdinal, ordinal, "Disposal")
        {
            this.disposables = disposables;
        }

        public override MethodResult Execute(object testClass)
        {
            this.disposables.DisposeAll();
            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}