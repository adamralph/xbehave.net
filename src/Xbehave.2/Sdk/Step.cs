// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides the implementation to execute each step.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class Step
    {
        private readonly string name;
        private readonly MethodInfo method;
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private readonly List<Action> teardowns = new List<Action>();

        public Step(string name, MethodInfo method)
        {
            this.name = name;
            this.method = method;
            this.MillisecondsTimeout = -1;
        }

        public virtual string Name
        {
            get { return this.name; }
        }

        public virtual MethodInfo Method
        {
            get { return this.method; }
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
    }
}