// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class Step : LongLivedMarshalByRefObject, ITest
    {
        private readonly ITest scenario;
        private readonly string displayName;

        public Step(ITest scenario, string displayName)
        {
            Guard.AgainstNullArgument("scenario", scenario);

            this.scenario = scenario;
            this.displayName = displayName;
        }

        public ITest Scenario
        {
            get { return this.scenario; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        public ITestCase TestCase
        {
            get { return this.scenario.TestCase; }
        }
    }
}
