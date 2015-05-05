// <copyright file="Scenario.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class Scenario : LongLivedMarshalByRefObject, ITest
    {
        private readonly IXunitTestCase scenarioOutline;
        private readonly string displayName;

        public Scenario(IXunitTestCase scenarioOutline, string displayName)
        {
            this.scenarioOutline = scenarioOutline;
            this.displayName = displayName;
        }

        public IXunitTestCase ScenarioOutline
        {
            get { return this.scenarioOutline; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        public ITestCase TestCase
        {
            get { return this.scenarioOutline; }
        }
    }
}
