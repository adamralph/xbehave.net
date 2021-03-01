using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xbehave.Execution
{
    public class ScenarioDiscoverer : TheoryDiscoverer
    {
        public ScenarioDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        [SuppressMessage(
            "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method.")]
        public override IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            discoveryOptions = discoveryOptions ?? throw new ArgumentNullException(nameof(discoveryOptions));

            yield return new ScenarioOutlineTestCase(
                this.DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod);
        }
    }
}
