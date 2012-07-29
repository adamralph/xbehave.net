// <copyright file="TestRunner.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal static class TestRunner
    {
        public static IEnumerable<MethodResult> Execute(IMethodInfo scenario)
        {
            object feature = scenario.IsStatic ? null : scenario.CreateInstance();
            return new ScenarioAttribute().CreateTestCommands(scenario).Select(step => step.Execute(feature)).ToArray();
        }
    }
}
