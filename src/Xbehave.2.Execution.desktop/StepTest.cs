// <copyright file="StepTest.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class StepTest : LongLivedMarshalByRefObject, IStepTest
    {
        private readonly IScenarioTestGroup testGroup;
        private readonly string displayName;

        public StepTest(IScenarioTestGroup testGroup, string displayName)
        {
            Guard.AgainstNullArgument("testGroup", testGroup);

            this.testGroup = testGroup;
            this.displayName = displayName;
        }

        public IScenarioTestGroup TestGroup
        {
            get { return this.testGroup; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        public ITestCase TestCase
        {
            get { return this.testGroup.TestCase; }
        }
    }
}
