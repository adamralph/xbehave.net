// <copyright file="Context.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;
    using Guard = Xbehave.Infra.Guard;

    internal class Context
    {
        private readonly ScenarioDefinition definition;
        private readonly IEnumerable<Step> steps;

        public Context(ScenarioDefinition definition, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("definition", definition);
            Guard.AgainstNullArgument("steps", steps);

            this.definition = definition;
            this.steps = steps;
        }

        public IEnumerable<ITestCommand> CreateTestCommands(int contextOrdinal)
        {
            var stepOrdinal = 1;
            foreach (var step in this.steps)
            {
                yield return new StepCommand(this.definition, contextOrdinal, stepOrdinal++, step);
            }

            // NOTE: this relies on the test runner executing each above command as soon as it is recieved, which TD.NET, R# and xunit.console all seem to do
            var disposables = CurrentScenario.GetDisposables();
            if (disposables.Any())
            {
                yield return new DisposalCommand(this.definition, contextOrdinal, stepOrdinal++, disposables.Reverse());
            }
        }
    }
}