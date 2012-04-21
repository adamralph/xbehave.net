// <copyright file="ScenarioFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    internal class ScenarioFactory
    {
        private readonly ITestFactory thenInIsolationTestFactory;
        private readonly ITestFactory thenTestFactory;
        private readonly ITestFactory thenSkipTestFactory;

        public ScenarioFactory(ITestNameFactory testNameFactory, IDisposer disposer)
        {
            this.thenInIsolationTestFactory = new ThenInIsolationTestFactory(testNameFactory, disposer);
            this.thenTestFactory = new ThenTestFactory(testNameFactory, disposer);
            this.thenSkipTestFactory = new ThenSkipTestFactory(testNameFactory);
        }

        public Scenario Create()
        {
            return new Scenario(this.thenInIsolationTestFactory, this.thenTestFactory, this.thenSkipTestFactory);
        }
    }
}
