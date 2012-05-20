// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Sdk.Infra;

    internal partial class Scenario
    {
        private readonly Queue<Step> steps = new Queue<Step>();

        public Step Enqueue(Step step)
        {
            this.steps.Enqueue(step);
            return step;
        }

        public IEnumerable<Context> CreateContexts(ScenarioDefinition definition)
        {
            var sharedContext = new Queue<Step>();
            foreach (var step in this.steps.DequeueAll())
            {
                if (step.InIsolation)
                {
                    yield return new Context(definition, sharedContext.Concat(step));
                }
                else
                {
                    sharedContext.Enqueue(step);
                    if (!this.steps.Any())
                    {
                        yield return new Context(definition, sharedContext);
                    }
                }
            }
        }
    }
}