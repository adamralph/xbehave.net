// <copyright file="Helper.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Sdk;

    internal static partial class Helper
    {
        public static Fluent.IStep AddStep(string text, Action body, StepType stepType)
        {
            return new Fluent.Step(CurrentScenario.AddStep(text, body, stepType));
        }
    }
}
