// <copyright file="Context.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Sdk.Infra;
    using Xunit.Sdk;

    internal class Context
    {
        private readonly ScenarioDefinition definition;
        private readonly IEnumerable<Step> steps;

        public Context(ScenarioDefinition definition, IEnumerable<Step> steps)
        {
            Require.NotNull(definition, "definition");
            Require.NotNull(steps, "steps");

            this.definition = definition;
            this.steps = steps;
        }

        public IEnumerable<ITestCommand> CreateTestCommands(int? ordinal)
        {
            var stepOrdinal = 1;
            var disposables = new Stack<IDisposable>();

            Action<IDisposable> handleResult = disposable =>
            {
                if (disposable != null)
                {
                    disposables.Push(disposable);
                }
            };

            foreach (var step in this.steps)
            {
                yield return new StepCommand(this.definition, ordinal, stepOrdinal++, step, handleResult);
            }

            if (disposables.Any())
            {
                yield return new DisposalCommand(this.definition, ordinal, stepOrdinal++, disposables.PopAll());
            }
        }
    }
}