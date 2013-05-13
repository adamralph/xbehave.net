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
        /// Represents all steps.
        /// </summary>
        All,

        /// <summary>
        /// Represents no steps.
        /// </summary>
        None,

        /// <summary>
        /// Represents "Given" steps.
        /// </summary>
        Given,

        /// <summary>
        /// Represents "When" steps.
        /// </summary>
        When,

        /// <summary>
        /// Represents "Then" steps.
        /// </summary>
        Then,

        /// <summary>
        /// Represents "And" steps.
        /// </summary>
        And,

        /// <summary>
        /// Represents "But" steps.
        /// </summary>
        But
    }
}