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
    public interface IStepDefinition
    {
        /// <summary>
        /// Gets or sets the natural language associated with step.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the body of the step.
        /// </summary>
        Func<IStepContext, Task> Body { get; set;  }

        /// <summary>
        /// Gets the teardowns to be invoked after the execution of the scenario in which the step participates.
        /// </summary>
        ICollection<Action> Teardowns { get; }

        /// <summary>
        /// Gets or sets the reason for skipping this step.
        /// </summary>
        string SkipReason { get; set; }

        /// <summary>
        /// Gets or sets the behavior of remaining steps if this step fails.
        /// </summary>
        RemainingSteps OnFailure { get; set; }
    }
}
