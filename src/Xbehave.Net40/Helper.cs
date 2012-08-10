// <copyright file="Helper.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Sdk;
    using Xbehave.Sdk.Infrastructure;

    internal static partial class Helper
    {
        public static Fluent.IStep AddStep(string stepType, string text, Action body)
        {
            var compressedStepType = stepType.CompressWhiteSpace();
            var prefix = string.IsNullOrEmpty(compressedStepType) ? compressedStepType : compressedStepType + " ";
            return new Fluent.Step(CurrentScenario.AddStep(prefix.MergeOrdinalIgnoreCase(text.CompressWhiteSpace()), body));
        }
    }
}
