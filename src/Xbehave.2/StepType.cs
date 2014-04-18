// <copyright file="StepType.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    /// <summary>
    /// Represents the type of a step.
    /// </summary>
    public enum StepType
    {
        /// <summary>
        /// Represents any step type.
        /// </summary>
        Any,

        /// <summary>
        /// Represents no step type.
        /// </summary>
        None,

        /// <summary>
        /// Represents a "Given" step.
        /// </summary>
        Given,

        /// <summary>
        /// Represents a "When" step.
        /// </summary>
        When,

        /// <summary>
        /// Represents a "Then" step.
        /// </summary>
        Then,

        /// <summary>
        /// Represents a "And" step.
        /// </summary>
        And,

        /// <summary>
        /// Represents a "But" step.
        /// </summary>
        But
    }
}