namespace Xbehave.Sdk
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Represents a scenario.
    /// </summary>
    public interface IScenario : ITest
    {
        /// <summary>
        /// Gets the display name of the scenario.
        /// </summary>
        new string DisplayName { get; }

        /// <summary>
        /// Gets the scenario outline this scenario belongs to.
        /// </summary>
        IXunitTestCase ScenarioOutline { get; }
    }
}
