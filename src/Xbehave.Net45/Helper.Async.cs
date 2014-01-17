// <copyright file="Helper.Async.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    internal static partial class Helper
    {
        public static Fluent.IStep AddStep(string text, Func<Task> body, StepType stepType)
        {
            return new Fluent.Step(CurrentScenario.AddStep(text, body, stepType));
        }
    }
}
