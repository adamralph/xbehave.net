// <copyright file="ContextFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;

    internal partial class ContextFactory
    {
        public IEnumerable<Context> CreateContexts(ScenarioDefinition definition, IEnumerable<Step> steps)
        {
            var sharedContext = new List<Step>();
            var pendingYield = false;
            foreach (var step in steps)
            {
                if (step.InIsolation)
                {
                    yield return new Context(definition, sharedContext.ToList().Concat(step));
                    pendingYield = false;
                }
                else
                {
                    sharedContext.Add(step);
                    pendingYield = true;
                }
            }

            if (pendingYield)
            {
                yield return new Context(definition, sharedContext);
            }
        }
    }
}