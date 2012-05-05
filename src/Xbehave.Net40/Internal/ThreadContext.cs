// <copyright file="ThreadContext.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal static class ThreadContext
    {
        private static ScenarioFactory scenarioFactory = new ScenarioFactory(new Disposer());

        [ThreadStatic]
        private static Scenario scenario;

        public static Scenario Scenario
        {
            get { return scenario ?? (scenario = scenarioFactory.Create()); }
        }

        // NOTE: I've tried to move this into Scenario, with the finally block clearing the steps but it just doesn't seem to work
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