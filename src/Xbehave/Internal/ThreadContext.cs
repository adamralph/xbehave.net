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
        [ThreadStatic]
        private static Scenario scenario;

        public static Scenario Scenario
        {
            get
            {
                if (scenario == null)
                {
                    scenario = new Scenario();
                }

                return scenario;
            }
        }

        public static IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method, Action registerSteps)
        {
            try
            {
                return Scenario.GetTestCommands(method, registerSteps);
            }
            finally
            {
                scenario = null;
            }
        }
    }
}