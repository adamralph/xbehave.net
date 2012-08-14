// <copyright file="ContextFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    public class ContextFactory
    {
        public IEnumerable<Context> CreateContexts(IMethodInfo method, IEnumerable<object> args, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("args", args);
            Guard.AgainstNullArgument("steps", steps);

            args = args.ToArray();

            var sharedContext = new List<Step>();
            var pendingYield = false;
            foreach (var step in steps)
            {
                if (step.InIsolation)
                {
                    yield return new Context(method, args, sharedContext.Concat(new[] { step }));
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
                yield return new Context(method, args, sharedContext);
            }
        }
    }
}