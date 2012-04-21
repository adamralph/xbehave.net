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

        public string CreateContext(IEnumerable<Step> givens, IEnumerable<Step> whens)
        {
            return string.Concat(this.testNameFactory.Create(givens.Concat(whens)), " { (shared context)");
        }

        public string Create(IEnumerable<Step> givens, IEnumerable<Step> whens, Step then)
        {
            return string.Concat(this.testNameFactory.Create(givens.Concat(whens)), " | ", this.testNameFactory.Create(then.AsEnumerable()));
        }

        public string CreateDisposal(IEnumerable<Step> givens, IEnumerable<Step> whens)
        {
            return string.Concat(this.testNameFactory.Create(givens.Concat(whens)), " } (disposal)");
        }
    }
}
