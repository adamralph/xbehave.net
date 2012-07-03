// <copyright file="Context.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;
    using Guard = Xbehave.Infra.Guard;

    internal class Context
    {
        [ThreadStatic]
        private static string failedStepCommandName;

        private readonly ScenarioDefinition definition;
        private readonly IEnumerable<Step> steps;

        public Context(ScenarioDefinition definition, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("definition", definition);
            Guard.AgainstNullArgument("steps", steps);

            this.definition = definition;
            this.steps = steps;
        }

        public static string FailedStepCommandName
        {
            get { return failedStepCommandName; }
            set { failedStepCommandName = value; }
        }

        public IEnumerable<ITestCommand> CreateTestCommands(int contextOrdinal)
        {
            FailedStepCommandName = null;
            var stepOrdinal = 1;
            foreach (var step in this.steps)
            {
                yield return new StepCommand(this.definition, contextOrdinal, stepOrdinal++, step);
            }

            // NOTE: this relies on the test runner executing each above yielded step command and below yielded disposal command as soon as it is recieved
            // TD.NET, R# and xunit.console all seem to do this
            var index = 0;
            while (true)
            {
                var disposables = CurrentScenario.GetDisposables();
                if (!disposables.Any())
                {
                    break;
                }

                // don't reverse odd disposables since their creation order has already been reversed by the previous command
                yield return new DisposalCommand(this.definition, contextOrdinal, stepOrdinal++, index % 2 == 0 ? disposables.Reverse() : disposables);

                ++index;
            }
        }
    }
}