// <copyright file="StepDefinition.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    internal class StepDefinition : IStepDefinition
    {
        private readonly string text;
        private readonly Func<IStepContext, Task> body;
        private readonly List<Action> teardowns = new List<Action>();

        public StepDefinition(string text, Func<IStepContext, Task> body)
        {
            this.text = text;
            this.body = body;
        }

        public virtual string Text
        {
            get { return this.text; }
        }

        public virtual Func<IStepContext, Task> Body
        {
            get { return this.body; }
        }

        public IReadOnlyList<Action> Teardowns
        {
            get { return this.teardowns.ToArray(); }
        }

        public string SkipReason { get; set; }

        public bool ContinueOnFailure { get; set; }

        public void Add(Action teardown)
        {
            if (teardown != null)
            {
                this.teardowns.Add(teardown);
            }
        }
    }
}
