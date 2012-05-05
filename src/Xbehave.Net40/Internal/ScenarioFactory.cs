// <copyright file="ScenarioFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using Xbehave.Infra;

    internal class ScenarioFactory
    {
        private readonly ICommandFactory agnosticCommandFactory;

        public ScenarioFactory(IDisposer disposer)
        {
            this.agnosticCommandFactory = new CommandFactory(disposer);
        }

        public Scenario Create()
        {
            return new Scenario(this.agnosticCommandFactory);
        }
    }
}
