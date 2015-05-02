// <copyright file="StepTest.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class StepTest : LongLivedMarshalByRefObject, ITest
    {
        private readonly ITestGroup testGroup;
        private readonly string displayName;

        public StepTest(ITestGroup testGroup, string displayName)
        {
            Guard.AgainstNullArgument("testGroup", testGroup);

            this.testGroup = testGroup;
            this.displayName = displayName;
        }

        public ITestGroup TestGroup
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
