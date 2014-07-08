// <copyright file="SyncStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Threading.Tasks;

    public class SyncStep : Step
    {
        private readonly Action body;

        public SyncStep(string name, Action body, object stepType)
            : base(name, stepType)
        {
            Guard.AgainstNullArgument("body", body);

            this.body = body;
        }

        public override Task RunAsync()
        {
            return Task.Factory.StartNew(this.body);
        }
    }
}