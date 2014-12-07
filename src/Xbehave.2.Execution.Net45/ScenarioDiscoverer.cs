// <copyright file="ScenarioDiscoverer.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioDiscoverer : IXunitTestCaseDiscoverer
    {
        [SuppressMessage(
            "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method.")]
        public IEnumerable<IXunitTestCase> Discover(ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            yield return new ScenarioOutline(testMethod);
        }
    }
}
