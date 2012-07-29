// <copyright file="TestRunner.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal static class TestRunner
    {
        public static IEnumerable<MethodResult> Execute(IMethodInfo scenario)
        {
            object feature = scenario.IsStatic ? null : scenario.CreateInstance();
            return CreateSteps(scenario).Select(step => Execute(scenario, step, feature)).ToArray();
        }

        public static IEnumerable<ITestCommand> CreateSteps(IMethodInfo scenario)
        {
            return new ScenarioAttribute().CreateTestCommands(scenario);
        }

        public static IMethodInfo CreateScenario(Action action)
        {
            return Reflector.Wrap(action.Method);
        }
        
        public static IMethodInfo CreateScenario<T1>(Action<T1> action)
        {
            return Reflector.Wrap(action.Method);
        }
        
        public static IMethodInfo CreateScenario<T1, T2>(Action<T1, T2> action)
        {
            return Reflector.Wrap(action.Method);
        }

        public static IMethodInfo CreateScenario<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            return Reflector.Wrap(action.Method);
        }

        private static MethodResult Execute(IMethodInfo method, ITestCommand step, object feature)
        {
            try
            {
                return step.Execute(feature);
            }
            catch (Exception ex)
            {
                return new FailedResult(method, ex, step.DisplayName);
            }
        }
    }
}
