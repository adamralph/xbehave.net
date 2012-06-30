// <copyright file="Helper.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Fluent;
    using Xbehave.Sdk;

    internal static class Helper
    {
        public static IStepDefinition AddStep(string stepType, string text, Action body)
        {
            return new StepDefinition(CurrentScenario.AddStep(new Step(stepType, text, body)));
        }
    }
}
