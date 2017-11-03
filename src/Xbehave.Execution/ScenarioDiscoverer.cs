namespace Xbehave.Execution
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public partial class ScenarioDiscoverer : TheoryDiscoverer
    {
        public ScenarioDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute, object[] dataRow) =>
                throw new System.NotImplementedException();

        [SuppressMessage(
            "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method.")]
        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        {
            Guard.AgainstNullArgument(nameof(discoveryOptions), discoveryOptions);

            yield return new ScenarioOutlineTestCase(
                this.DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod);
        }
    }
}
