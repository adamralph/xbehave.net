// <copyright file="Step.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public abstract class Step
    {
        protected readonly MethodInfo MethodInfo;
        protected readonly object Target;
        protected readonly List<Action> Teardowns = new List<Action>();
        private readonly string name;
        private readonly object stepType;

        public Step(string name, MethodInfo methodInfo, object target, object stepType)
        {
            Guard.AgainstNullArgument("methodInfo", methodInfo);

            this.name = name;
            this.MethodInfo = methodInfo;
            this.Target = target;
            this.stepType = stepType;
        }

        public virtual string Name
        {
            get { return this.name; }
        }

        public object StepType
        {
            get { return this.stepType; }
        }

        public string SkipReason { get; set; }

        public bool InIsolation { get; set; }

        public int MillisecondsTimeout { get; set; }

        public void AddTeardown(Action teardown)
        {
            if (teardown != null)
            {
                this.Teardowns.Add(teardown);
            }
        }

        public abstract void Execute();

        protected object ExecuteMethodInfo()
        {
            return this.MethodInfo.Invoke(this.Target, null);
        }
    }
}