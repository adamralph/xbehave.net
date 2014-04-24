// <copyright file="AsyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading.Tasks;

    public class AsyncStep : Step
    {
        private readonly Func<Task> body;

        public AsyncStep(string name, Func<Task> body, object stepType)
            : base(name, stepType)
        {
            Guard.AgainstNullArgument("body", body);

            this.body = body;
        }
    }
}
