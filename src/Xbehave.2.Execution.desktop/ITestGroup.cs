// <copyright file="ITestGroup.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Abstractions;

    public interface ITestGroup
    {
        ITestCase TestCase { get; }

        string DisplayName { get; }
    }
}
