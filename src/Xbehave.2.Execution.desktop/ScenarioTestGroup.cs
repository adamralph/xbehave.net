// <copyright file="ScenarioTestGroup.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioTestGroup : IScenarioTestGroup
    {
        private readonly IXunitTestCase testCase;
        private readonly string displayName;

        public ScenarioTestGroup(IXunitTestCase testCase, string displayName)
        {
            this.testCase = testCase;
            this.displayName = displayName;
        }

        public IXunitTestCase TestCase
        {
            get { return this.testCase; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        ITestCase ITestGroup.TestCase
        {
            get { return this.TestCase; }
        }
    }
}
