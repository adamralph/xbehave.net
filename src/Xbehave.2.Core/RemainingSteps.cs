// <copyright file="RemainingSteps.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    /// <summary>
    /// Indicates the behavior of remaining steps when a step fails.
    /// </summary>
    public enum RemainingSteps
    {
        /// <summary>
        /// Skip remaining steps.
        /// </summary>
        Skip = 0,

        /// <summary>
        /// Run remaining steps.
        /// </summary>
        Run = 1,
    }
}
