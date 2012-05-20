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
        public IEnumerable<ITestCommand> Create(ScenarioDefinition definition, int? contextOrdinal, IEnumerable<Step> steps)
        {
            var ordinal = 1;
            var disposables = new Stack<IDisposable>();
            foreach (var step in steps)
            {
                yield return new StepCommand(definition, contextOrdinal, ordinal++, step, result => disposables.PushIfNotNull(result));
            }

            if (disposables.Any())
            {
                yield return new DisposalCommand(definition, contextOrdinal, ordinal++, disposables.Unwind());
            }
        }
    }
}