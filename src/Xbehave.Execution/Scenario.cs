namespace Xbehave.Execution
{
    using Xbehave.Sdk;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class Scenario : LongLivedMarshalByRefObject, IScenario
    {
        public Scenario(IXunitTestCase scenarioOutline, string displayName)
        {
            this.ScenarioOutline = scenarioOutline;
            this.DisplayName = displayName;
        }

        public IXunitTestCase ScenarioOutline { get; }

        public string DisplayName { get; }

        public ITestCase TestCase => this.ScenarioOutline;
    }
}
