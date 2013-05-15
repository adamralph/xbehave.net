// <copyright file="ContinueOnFailureAfterAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    /// <summary>
    /// Allows step execution within a scenario to continue after a failed step.
    /// <example>
    /// To allow step execution to continue after any failed step:
    /// <code>[ContinueOnFailureAfter(StepType.Any)]</code>.
    /// </example>
    /// <example>
    /// To allow step execution to continue after a failed step but only after the execution of the first "Then" step, use
    /// <code>[ContinueOnFailureAfter(StepType.Then)]</code>.
    /// </example>
    /// The default setting is <c>StepType.None</c>.
    /// This attribute can be applied at the level of method (scenario), class (feature) or assembly (product).
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ContinueOnFailureAfterAttribute : Attribute
    {
        private readonly StepType stepType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueOnFailureAfterAttribute"/> class.
        /// </summary>
        /// <param name="stepType">Type of the step.</param>
        public ContinueOnFailureAfterAttribute(StepType stepType)
        {
            this.stepType = stepType;
        }

        /// <summary>
        /// Gets the type of the step.
        /// </summary>
        /// <value>
        /// The type of the step.
        /// </value>
        public StepType StepType
        {
            get { return this.stepType; }
        }
    }
}
