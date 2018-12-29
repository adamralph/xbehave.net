namespace Xbehave.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioTestCase : XunitTestCase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public ScenarioTestCase()
        {
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public ScenarioTestCase(IMessageSink diagnosticMessageSink,
                                TestMethodDisplay defaultMethodDisplay,
                                TestMethodDisplayOptions defaultMethodDisplayOptions,
                                ITestMethod testMethod,
                                object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
        }
    }
}
