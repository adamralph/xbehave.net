// <copyright file="BackgroundDiscoverer.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System.Collections.Generic;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class BackgroundDiscoverer : IXunitTestCaseDiscoverer
    {
        public BackgroundDiscoverer(IMessageSink diagnosticMessageSink)
        {
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            yield break;
        }
    }
}
