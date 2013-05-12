// <copyright file="ShouldFailFastBeforeAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;

    /// <summary>
    /// This attribute is used to allow certain steps to execute regardless of a previous failure.
    /// <example>
    /// [ShouldFailFastBefore(StepType.Then)]
    /// </example>
    /// In the above example, if a failure occurs before the first "Then" step, no further steps will execute. However,
    /// if a failure occurs during or after the first "Then" step, the following steps will still execute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ShouldFailFastBeforeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShouldFailFastBeforeAttribute" /> class with the specified <see cref="Xbehave.Sdk.StepType"/>.
        /// </summary>
        /// <param name="stepType">The step type.</param>
        public ShouldFailFastBeforeAttribute(StepType stepType)
        {
            this.StepType = stepType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShouldFailFastBeforeAttribute" /> class with the specified custom step type.
        /// </summary>
        /// <param name="stepType">The step type.</param>
        public ShouldFailFastBeforeAttribute(object stepType)
        {
            this.StepType = stepType;
        }

        /// <summary>
        /// Gets or sets the step type.
        /// </summary>
        public object StepType { get; set; }
    }
}