// <copyright file="SharedContextTestNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    internal class SharedContextTestNameFactory : ISharedContextTestNameFactory
    {
        private readonly ITestNameFactory testNameFactory;

        public SharedContextTestNameFactory(ITestNameFactory testNameFactory)
        {
            this.testNameFactory = testNameFactory;
        }

        public string CreateContext(IEnumerable<Step> steps)
        {
            return string.Concat(this.testNameFactory.Create(steps), " { (shared context)");
        }

        public string Create(IEnumerable<Step> contextSteps, Step step)
        {
            return string.Concat(this.testNameFactory.Create(contextSteps), " | ", this.testNameFactory.Create(step.AsEnumerable()));
        }

        public string CreateDisposal(IEnumerable<Step> steps)
        {
            return string.Concat(this.testNameFactory.Create(steps), " } (disposal)");
        }
    }
}
