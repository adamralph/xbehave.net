// <copyright file="ITestGroupTest.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using Xunit.Abstractions;

    public interface ITestGroupTest : ITest
    {
        ITestGroup TestGroup { get; }
    }
}
