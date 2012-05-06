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
        public IEnumerable<ITestCommand> Create(IEnumerable<Step> steps, MethodCall call, int? contextOrdinal)
        {
            var disposables = new Stack<IDisposable>();
            Action<IDisposable> handleResult = result =>
            {
                if (result != null)
                {
                    disposables.Push(result);
                }
            };

            var ordinal = 1;
            foreach (var step in steps)
            {
                yield return new StepCommand(call, contextOrdinal, ordinal++, step, handleResult);
            }

            if (disposables.Any())
            {
                yield return new DisposalCommand(call, contextOrdinal, ordinal++, disposables.Unwind());
            }
        }
    }
}