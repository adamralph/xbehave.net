// <copyright file="CommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal class CommandFactory : ICommandFactory
    {
        public IEnumerable<ITestCommand> Create(IEnumerable<Step> steps, MethodCall call, string context)
        {
            var stepOrdinal = 1;
            var disposables = new Stack<IDisposable>();
            foreach (var step in steps)
            {
                yield return new StepCommand(call, stepOrdinal++, step, context, result => disposables.Push(result));
            }

            if (disposables.Any(disposable => disposable != null))
            {
                yield return new DisposalCommand(call, stepOrdinal++, context, disposables.Unwind());
            }
        }
    }
}