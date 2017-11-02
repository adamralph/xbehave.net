// <copyright file="IStepDefinition.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the definition of a step within a scenario.
    /// </summary>
    /// <remarks>This is the type used for step filters.</remarks>
    public interface IStepDefinition : IStepBuilder
    {
        /// <summary>
        /// Gets or sets the natural language associated with step.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the body of the step.
        /// </summary>
        Func<IStepContext, Task> Body { get; set; }

        /// <summary>
        /// Gets the teardowns to be invoked after the execution of the scenario in which the step participates.
        /// </summary>
        ICollection<Func<IStepContext, Task>> Teardowns { get; }

        /// <summary>
        /// Gets or sets the reason for skipping this step.
        /// </summary>
        string SkipReason { get; set; }

        /// <summary>
        /// Gets or sets the behavior of remaining steps if this step fails.
        /// </summary>
        RemainingSteps FailureBehavior { get; set; }

        /// <summary>
        /// Indicates that the step will not be executed.
        /// </summary>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        /// <remarks>If the <paramref name="reason"/> is <c>null</c> then the step will not be skipped.</remarks>
        new IStepDefinition Skip(string reason);

        /// <summary>
        /// Declares a teardown action (related to this step and/or previous steps) which will be executed
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        new IStepDefinition Teardown(Func<IStepContext, Task> action);

        /// <summary>
        /// Defines the behavior of remaining steps if this step fails.
        /// </summary>
        /// <param name="behavior">The behavior of remaining steps.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        new IStepDefinition OnFailure(RemainingSteps behavior);
    }
}
