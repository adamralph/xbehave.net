// <copyright file="IStepDefinition.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the natural language associated with a step, the body of the step,
    /// the teardowns to be invoked after the execution of the scenario in which the step participates,
    /// the objects to be disposed after the execution of the scenario in which the step participates and
    /// a reason for skipping the step.
    /// </summary>
    public interface IStepDefinition
    {
        /// <summary>
        /// Gets the natural language associated with step.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the body of the step.
        /// </summary>
        Func<IStepContext, Task> Body { get; }

        /// <summary>
        /// Gets the teardowns to be invoked after the execution of the scenario in which the step participates.
        /// </summary>
        IReadOnlyList<Action> Teardowns { get; }

        /// <summary>
        /// Gets or sets the reason for skipping this step.
        /// </summary>
        string SkipReason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to continue execution of remaining steps even if this step fails.
        /// </summary>
        bool ContinueOnFailure { get; set; }

        /// <summary>
        /// Adds a teardown to be invoked after the execution of the scenario in which the step participates.
        /// </summary>
        /// <param name="teardown">The body of the teardown.</param>
        void Add(Action teardown);
    }
}
