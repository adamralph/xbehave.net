// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal partial class Scenario
    {
        private readonly ICommandFactory commandFactory;

        private readonly Queue<Step> steps = new Queue<Step>();

        public Scenario(ICommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        public Step AddStep(Step step)
        {
            return this.steps.EnqueueAndReturn(step);
        }

        public IEnumerable<ITestCommand> GetTestCommands(MethodCall call)
        {
            var sharedContext = new Queue<Step>();
            var isolatedContextOrdinal = 1;
            foreach (var step in this.steps.DequeueAll())
            {
                if (!step.InIsolation)
                {
                    sharedContext.Enqueue(step);
                }

                if (step.InIsolation)
                {
                    var context = (this.steps.Any() || isolatedContextOrdinal > 1)
                        ? "context " + (isolatedContextOrdinal++).ToString(CultureInfo.InvariantCulture)
                        : null;

                    foreach (var command in this.commandFactory.Create(sharedContext.Concat(step.AsEnumerable()), call, context))
                    {
                        yield return command;
                    }
                }
                else if (!this.steps.Any())
                {
                    var context = isolatedContextOrdinal > 1 ? "shared context" : null;
                    foreach (var command in this.commandFactory.Create(sharedContext, call, context))
                    {
                        yield return command;
                    }
                }
            }
        }
    }
}