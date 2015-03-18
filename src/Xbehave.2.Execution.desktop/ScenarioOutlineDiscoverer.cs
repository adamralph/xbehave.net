// <copyright file="ScenarioOutlineDiscoverer.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioOutlineDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink diagnosticMessageSink;

        public ScenarioOutlineDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        [SuppressMessage(
            "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method.")]
        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            Guard.AgainstNullArgument("discoveryOptions", discoveryOptions);

            yield return new ScenarioOutline(
                this.diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
        }
    }
}
