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
        private static ScenarioFactory scenarioFactory = new ScenarioFactory(new TestNameFactory(), new Disposer());

        [ThreadStatic]
        private static Scenario scenario;

        public static Scenario Scenario
        {
            get { return scenario ?? (scenario = scenarioFactory.Create()); }
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
    }
}