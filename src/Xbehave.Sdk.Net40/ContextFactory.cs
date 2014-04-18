// <copyright file="ContextFactory.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Collections.Generic;
    using System.Linq;

    public class ContextFactory
    {
        public IEnumerable<Context> CreateContexts(MethodCall methodCall, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("steps", steps);

            var sharedContext = new List<Step>();
            var pendingYield = false;
            foreach (var step in steps)
            {
                if (step.InIsolation)
                {
                    yield return new Context(methodCall, sharedContext.Concat(new[] { step }));
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
                yield return new Context(methodCall, sharedContext);
            }
        }
    }
}