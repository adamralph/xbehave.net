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
        private readonly List<Action> teardowns = new List<Action>();

        public string Text { get; set; }

        public Func<IStepContext, Task> Body { get; set; }

        public ICollection<Action> Teardowns
        {
            get { return this.teardowns; }
        }

        public string SkipReason { get; set; }

        public RemainingSteps FailureBehavior { get; set; }

        public bool IsBackgroundStep { get; set; }

        public IStepDefinition Skip(string reason)
        {
            this.SkipReason = reason;
            return this;
        }

        public IStepDefinition Teardown(Action action)
        {
            this.Teardowns.Add(action);
            return this;
        }

        public IStepDefinition Failure(RemainingSteps behavior)
        {
            this.FailureBehavior = behavior;
            return this;
        }

        IStepBuilder IStepBuilder.Skip(string reason)
        {
            return this.Skip(reason);
        }

        IStepBuilder IStepBuilder.Teardown(Action action)
        {
            return this.Teardown(action);
        }

        IStepBuilder IStepBuilder.Failure(RemainingSteps behavior)
        {
            return this.Failure(behavior);
        }
    }
}
