// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the natural language associated with a step, the body of a step,
    /// the teardowns to be invoked after the execution of the scenario in which the step participates,
    /// the objects to be disposed after the execution of the scenario in which the step participates and
    /// a reason for skipping this step.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class Step
    {
        private readonly string text;
        private readonly Func<object> body;
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private readonly List<Action> teardowns = new List<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class.
        /// </summary>
        /// <param name="text">The natural language associated with step.</param>
        /// <param name="body">The body of the step.</param>
        public Step(string text, Action body)
            : this(text)
        {
            this.body = () =>
            {
                body();
                return null;
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Step"/> class.
        /// </summary>
        /// <param name="text">The natural language associated with step.</param>
        /// <param name="body">The body of the step.</param>
        public Step(string text, Func<Task> body)
            : this(text)
        {
            this.body = body;
        }

        private Step(string text)
        {
            this.text = text;
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
        public virtual Func<object> Body
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
        /// Gets the objects to be disposed after the execution of the scenario in which the step participates.
        /// </summary>
        public IReadOnlyList<IDisposable> Disposables
        {
            get { return this.disposables.ToArray(); }
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

        /// <summary>
        /// Adds an object to be disposed after the execution of the scenario in which the step participates.
        /// </summary>
        /// <param name="disposable">A disposable object.</param>
        public void Add(IDisposable disposable)
        {
            if (disposable != null)
            {
                this.disposables.Add(disposable);
            }
        }
    }
}
