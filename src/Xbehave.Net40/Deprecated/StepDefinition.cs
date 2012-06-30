// <copyright file="StepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;

    internal partial class StepDefinition : IStepDefinition
    {
        public IStepDefinition When(string message, Action body, Action dispose)
        {
            return message.When(body).Teardown(dispose);
        }

        public IStepDefinition Then(string message, Action body, Action dispose)
        {
            return message.Then(body).Teardown(dispose);
        }

        public IStepDefinition And(string message, Action body, Action dispose)
        {
            return message.And(body).Teardown(dispose);
        }

        public IStepDefinition But(string message, Action body, Action dispose)
        {
            return message.But(body).Teardown(dispose);
        }
    }
}
