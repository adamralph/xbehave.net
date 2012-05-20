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
        private readonly List<Step> steps = new List<Step>();

        public Step AddStep(Step step)
        {
            this.steps.Add(step);
            return step;
        }

        public IEnumerable<Context> CreateContexts(ScenarioDefinition definition)
        {
            var sharedContext = new List<Step>();
            var isPendingYield = false;
            foreach (var step in this.steps)
            {
                if (step.InIsolation)
                {
                    yield return new Context(definition, sharedContext.ToList().Concat(step));
                    isPendingYield = false;
                }
                else
                {
                    sharedContext.Add(step);
                    isPendingYield = true;
                }
            }

            if (isPendingYield)
            {
                yield return new Context(definition, sharedContext);
            }
        }
    }
}