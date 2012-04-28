// <copyright file="ScenarioFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using Xbehave.Infra;

    internal class ScenarioFactory
    {
        private readonly ICommandFactory thenInIsolationTestFactory;
        private readonly ICommandFactory thenTestFactory;
        private readonly ICommandFactory thenSkipTestFactory;

        public ScenarioFactory(ICommandNameFactory testNameFactory, IDisposer disposer)
        {
            this.thenInIsolationTestFactory = new ThenInIsolationCommandFactory(testNameFactory, disposer);
            this.thenTestFactory = new ThenCommandFactory(new SharedContextCommandNameFactory(testNameFactory), disposer);
            this.thenSkipTestFactory = new ThenSkipCommandFactory(testNameFactory);
        }

        public Scenario Create()
        {
            return new Scenario(this.thenInIsolationTestFactory, this.thenTestFactory, this.thenSkipTestFactory);
        }
    }
}
