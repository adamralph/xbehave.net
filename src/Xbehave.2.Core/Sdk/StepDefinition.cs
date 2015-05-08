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

        public bool ContinueOnFailure { get; set; }

        public bool IsBackgroundStep { get; set; }
    }
}
