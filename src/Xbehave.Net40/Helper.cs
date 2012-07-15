// <copyright file="Helper.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Sdk;

    internal static partial class Helper
    {
        public static Fluent.IStep AddStep(string stepType, string text, Action body)
        {
            return new Fluent.Step(CurrentScenario.AddStep(stepType, text, body));
        }
    }
}
