// <copyright file="IScenarioTestGroup.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Sdk;

    public interface IScenarioTestGroup : ITestGroup
    {
        new IXunitTestCase TestCase { get; }
    }
}
