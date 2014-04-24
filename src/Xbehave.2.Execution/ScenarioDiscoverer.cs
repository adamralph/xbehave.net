// <copyright file="ScenarioDiscoverer.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System.Collections.Generic;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    ////public class ScenarioDiscoverer : FactDiscoverer
    ////{
    ////}

    public class ScenarioDiscoverer : IXunitDiscoverer
    {
        private readonly FactDiscoverer discoverer = new FactDiscoverer();

        public IEnumerable<XunitTestCase> Discover(
            ITestCollection testCollection,
            IAssemblyInfo assembly,
            ITypeInfo testClass,
            IMethodInfo testMethod,
            IAttributeInfo factAttribute)
        {
            return this.discoverer.Discover(testCollection, assembly, testClass, testMethod, factAttribute);
        }
    }
}
