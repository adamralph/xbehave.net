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

        public static Fluent.IStep AddStep(string text, Action<IStepContext> body, StepType stepType)
        {
            var context = new StepContext();
            var step = CurrentScenario.AddStep(text, () => body(context), stepType);
            context.Assign(step);
            return new Fluent.Step(step);
        }
    }
}
