// <copyright file="Helper.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Fluent;
    using Xbehave.Sdk;

    internal static class Helper
    {
        public static IStepDefinition AddStep(string stepType, string text, Func<IDisposable> body, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentScenario.AddStep(new Step(stepType, text, body, inIsolation, skipReason)));
        }

        public static IStepDefinition AddStep(string stepType, string text, Action body, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentScenario.AddStep(new Step(stepType, text, body, inIsolation, skipReason)));
        }

        public static IStepDefinition AddStep(string stepType, string text, Func<IEnumerable<IDisposable>> body, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentScenario.AddStep(new Step(stepType, text, body, inIsolation, skipReason)));
        }

        public static IStepDefinition AddStep(string stepType, string text, Action body, Action dispose, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentScenario.AddStep(new Step(stepType, text, body, dispose, inIsolation, skipReason)));
        }
    }
}
