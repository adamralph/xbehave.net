// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class Step : LongLivedMarshalByRefObject, IStep
    {
        private readonly IScenario scenario;
        private readonly string displayName;

        public Step(IScenario scenario, string displayName)
        {
            Guard.AgainstNullArgument("scenario", scenario);

            this.scenario = scenario;
            this.displayName = displayName;
        }

        public IScenario Scenario
        {
            get { return this.scenario; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        public ITestCase TestCase
        {
            get { return this.scenario.ScenarioOutline; }
        }
    }
}
