// <copyright file="ThreadContext.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal static class ThreadContext
    {
        private static ITestNameFactory testNameFactory = new TestNameFactory();
        private static IDisposer disposer = new Disposer();

        [ThreadStatic]
        private static Scenario scenario;

        public static Scenario Scenario
        {
            get { return scenario ?? (scenario = CreateScenario()); }
        }

        public static IEnumerable<ITestCommand> CreateTestCommands(IMethodInfo method, Action registerSteps)
        {
            try
            {
                registerSteps();
                return Scenario.GetTestCommands(method);
            }
            finally
            {
                scenario = null;
            }
        }

        private static Scenario CreateScenario()
        {
            return new Scenario(
                new ThenInIsolationTestFactory(testNameFactory, disposer), new ThenTestFactory(testNameFactory, disposer), new ThenSkipTestFactory(testNameFactory));
        }
    }
}