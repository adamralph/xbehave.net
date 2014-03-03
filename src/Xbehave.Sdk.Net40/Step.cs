// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Provides the implementation to execute each step.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public abstract partial class Step
    {
        private readonly string name;
        private readonly object stepType;
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private readonly List<Action> teardowns = new List<Action>();

        public Step(string name, object stepType)
        {
            this.name = name;
            this.stepType = stepType;
            this.MillisecondsTimeout = -1;
        }

        public virtual string Name
        {
            get { return this.name; }
        }

        public object StepType
        {
            get { return this.stepType; }
        }

        public IEnumerable<IDisposable> ExtractDisposables
        {
            get
            {
                var extracted = this.disposables.ToArray();
                this.disposables.Clear();
                return extracted;
            }
        }

        public IEnumerable<Action> Teardowns
        {
            get { return this.teardowns.Select(x => x); }
        }

        public string SkipReason { get; set; }

        public bool InIsolation { get; set; }

        public int MillisecondsTimeout { get; set; }

        public void AddDisposable(IDisposable disposable)
        {
            if (disposable != null)
            {
                this.disposables.Add(disposable);
            }
        }

        public void AddTeardown(Action teardown)
        {
            if (teardown != null)
            {
                this.teardowns.Add(teardown);
            }
        }

        public abstract void Execute();
    }
}