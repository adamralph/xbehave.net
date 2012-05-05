// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal partial class Scenario
    {
        private readonly ICommandFactory commandFactory;

        private readonly Queue<Step> steps = new Queue<Step>();

        public Scenario(ICommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        public Step AddStep(Step step)
        {
            return this.steps.EnqueueAndReturn(step);
        }

        public IEnumerable<ITestCommand> GetTestCommands(MethodCall call)
        {
            return this.commandFactory.Create(this.steps, call);
        }
    }
}