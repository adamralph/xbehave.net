// <copyright file="CommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Sdk.Infra;
    using Xunit.Sdk;

    internal class CommandFactory : ICommandFactory
    {
        public IEnumerable<ITestCommand> Create(IEnumerable<Step> steps, MethodCall call, int? contextOrdinal)
        {
            var ordinal = 1;
            var disposables = new Stack<IDisposable>();
            foreach (var step in steps)
            {
                yield return new StepCommand(call, contextOrdinal, ordinal++, step, result => disposables.PushIfNotNull(result));
            }

            if (disposables.Any())
            {
                yield return new DisposalCommand(call, contextOrdinal, ordinal++, disposables.Unwind());
            }
        }
    }
}