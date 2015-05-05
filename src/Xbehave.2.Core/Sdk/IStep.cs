// <copyright file="IStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using Xunit.Abstractions;

    /// <summary>
    /// Represents a single step in a scenario.
    /// </summary>
    public interface IStep : ITest
    {
        /// <summary>
        /// Gets the scenario this step belongs to.
        /// </summary>
        IScenario Scenario { get; }
    }
}
