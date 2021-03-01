using System;
using System.Diagnostics.CodeAnalysis;
using Xbehave.Sdk;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Execution
{
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class StepTest : LongLivedMarshalByRefObject, IStep
    {
        public StepTest(IScenario scenario, string displayName)
        {
            this.Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
            this.DisplayName = displayName;
        }

        public IScenario Scenario { get; }

        public string DisplayName { get; }

        public ITestCase TestCase => this.Scenario.ScenarioOutline;
    }
}
