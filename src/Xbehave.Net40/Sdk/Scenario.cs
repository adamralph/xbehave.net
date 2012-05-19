// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Sdk.Infra;
    using Xunit.Sdk;

    internal partial class Scenario
    {
        private readonly ICommandFactory commandFactory;

        private readonly Queue<Step> steps = new Queue<Step>();

        public Scenario(ICommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        public Step Enqueue(Step step)
        {
            return this.steps.EnqueueAndReturn(step);
        }

        public IEnumerable<ITestCommand> GetTestCommands(MethodCall call)
        {
            var sharedContext = new Queue<Step>();
            var contextOrdinal = 1;
            foreach (var step in this.steps.DequeueAll())
            {
                if (!step.InIsolation)
                {
                    sharedContext.Enqueue(step);
                }

                if (step.InIsolation)
                {
                    var ordinal = (this.steps.Any() || contextOrdinal > 1) ? (int?)contextOrdinal++ : null;
                    foreach (var command in this.commandFactory.Create(sharedContext.Concat(step), call, ordinal))
                    {
                        yield return command;
                    }
                }
                else if (!this.steps.Any())
                {
                    var ordinal = contextOrdinal > 1 ? (int?)contextOrdinal : null;
                    foreach (var command in this.commandFactory.Create(sharedContext, call, ordinal))
                    {
                        yield return command;
                    }
                }
            }
        }
    }
}