// <copyright file="StepDefinition.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the natural language associated with a step, the body of the step,
    /// the teardowns to be invoked after the execution of the scenario in which the step participates,
    /// the objects to be disposed after the execution of the scenario in which the step participates and
    /// a reason for skipping this step.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class StepDefinition
    {
        private readonly string text;
        private readonly Func<IStepContext, Task> body;
        private readonly List<Action> teardowns = new List<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StepDefinition"/> class.
        /// </summary>
        /// <param name="text">The natural language associated with step.</param>
        /// <param name="body">The body of the step.</param>
        public StepDefinition(string text, Func<IStepContext, Task> body)
        {
            this.text = text;
            this.body = body;
        }

        /// <summary>
        /// Gets the natural language associated with step.
        /// </summary>
        public virtual string Text
        {
            get { return this.text; }
        }

        /// <summary>
        /// Gets the body of the step.
        /// </summary>
        public virtual Func<IStepContext, Task> Body
        {
            get { return this.body; }
        }

        /// <summary>
        /// Gets the teardowns to be invoked after the execution of the scenario in which the step participates.
        /// </summary>
        public IReadOnlyList<Action> Teardowns
        {
            get { return this.teardowns.ToArray(); }
        }

        /// <summary>
        /// Gets or sets the reason for skipping this step.
        /// </summary>
        public string SkipReason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to continue execution of remaining steps even if this step fails.
        /// </summary>
        public bool ContinueOnFailure { get; set; }

        /// <summary>
        /// Adds a teardown to be invoked after the execution of the scenario in which the step participates.
        /// </summary>
        /// <param name="teardown">The body of the teardown.</param>
        public void Add(Action teardown)
        {
            if (teardown != null)
            {
                this.teardowns.Add(teardown);
            }
        }
    }
}
