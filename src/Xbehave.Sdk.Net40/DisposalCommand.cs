// <copyright file="DisposalCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Sdk.Infrastructure;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    public class DisposalCommand : CommandBase
    {
        private readonly IEnumerable<IDisposable> disposables;

        public DisposalCommand(IMethodInfo method, string scenarioName, int contextOrdinal, int stepOrdinal, IEnumerable<IDisposable> disposables)
            : base(method, scenarioName, contextOrdinal, stepOrdinal, "Disposal")
        {
            Guard.AgainstNullArgument("disposables", disposables);

            this.disposables = disposables;
        }

        public override MethodResult Execute(object testClass)
        {
            this.disposables.DisposeAll();
            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}