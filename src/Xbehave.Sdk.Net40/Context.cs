// <copyright file="Context.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Sdk.Infrastructure;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    public class Context
    {
        [ThreadStatic]
        private static string failedStepName;

        private readonly ScenarioDefinition definition;
        private readonly IEnumerable<Step> steps;

        public Context(ScenarioDefinition definition, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("definition", definition);
            Guard.AgainstNullArgument("steps", steps);

            this.definition = definition;
            this.steps = steps;
        }

        public static string FailedStepName
        {
            get { return failedStepName; }
            set { failedStepName = value; }
        }

        public IEnumerable<ITestCommand> CreateTestCommands(int contextOrdinal)
        {
            FailedStepName = null;
            var stepOrdinal = 1;
            foreach (var step in this.steps)
            {
                yield return new StepCommand(this.definition.Method, this.definition.ToString(), contextOrdinal, stepOrdinal++, step.Name.AttemptFormatInvariantCulture(this.definition.Args), step);
            }

            // NOTE: this relies on the test runner executing each above yielded step command and below yielded disposal command as soon as it is recieved
            // TD.NET, R# and xunit.console all seem to do this
            FailedStepName = null;
            var index = 0;
            while (true)
            {
                var disposables = CurrentScenario.ExtractDisposables();
                if (!disposables.Any())
                {
                    break;
                }

                // don't reverse odd disposables since their creation order has already been reversed by the previous command
                var disposalStep = new DisposalStep(index % 2 == 0 ? disposables.Reverse() : disposables);
                yield return new StepCommand(this.definition.Method, this.definition.ToString(), contextOrdinal, stepOrdinal++, disposalStep.Name, disposalStep);

                ++index;
            }
        }
    }
}