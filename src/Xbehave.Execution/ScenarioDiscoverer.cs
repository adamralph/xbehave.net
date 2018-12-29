namespace Xbehave.Execution
{
    using System.Collections.Generic;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioDiscoverer : TheoryDiscoverer
    {
        public ScenarioDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute, object[] dataRow) =>
            new[] { new ScenarioTestCase(this.DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, dataRow) };

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute) =>
            new[] { new ScenarioOutlineTestCase(this.DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod) };

        protected override IXunitTestCase CreateTestCaseForNoData(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod) =>
            new ScenarioTestCase(this.DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod);
    }
}
