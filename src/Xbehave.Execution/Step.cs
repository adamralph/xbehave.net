namespace Xbehave.Execution
{
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Sdk;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class Step : LongLivedMarshalByRefObject, IStep
    {
        public Step(IScenario scenario, string displayName)
        {
            Guard.AgainstNullArgument(nameof(scenario), scenario);

            this.Scenario = scenario;
            this.DisplayName = displayName;
        }

        public IScenario Scenario { get; }

        public string DisplayName { get; }

        public ITestCase TestCase => this.Scenario.ScenarioOutline;
    }
}
