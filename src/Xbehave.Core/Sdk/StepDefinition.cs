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
        private readonly List<Func<IStepContext, Task>> teardowns = new List<Func<IStepContext, Task>>();

        public string Text { get; set; }

        public Func<IStepContext, Task> Body { get; set; }

        public ICollection<Func<IStepContext, Task>> Teardowns
        {
            get { return this.teardowns; }
        }

        public string SkipReason { get; set; }

        public RemainingSteps FailureBehavior { get; set; }

        public IStepDefinition Skip(string reason)
        {
            this.SkipReason = reason;
            return this;
        }

        public IStepDefinition Teardown(Func<IStepContext, Task> action)
        {
            this.Teardowns.Add(action);
            return this;
        }

        public IStepDefinition OnFailure(RemainingSteps behavior)
        {
            this.FailureBehavior = behavior;
            return this;
        }

        IStepBuilder IStepBuilder.Skip(string reason)
        {
            return this.Skip(reason);
        }

        IStepBuilder IStepBuilder.Teardown(Func<IStepContext, Task> action)
        {
            return this.Teardown(action);
        }

        IStepBuilder IStepBuilder.OnFailure(RemainingSteps behavior)
        {
            return this.OnFailure(behavior);
        }
    }
}
