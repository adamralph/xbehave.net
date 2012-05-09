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
        public static IStepDefinition Enqueue(string prefix, string message, Func<IDisposable> body, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(prefix, message, body, inIsolation, skipReason)));
        }

        public static IStepDefinition Enqueue(string prefix, string message, Action body, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(prefix, message, body, inIsolation, skipReason)));
        }

        public static IStepDefinition Enqueue(string prefix, string message, Func<IEnumerable<IDisposable>> body, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(prefix, message, body, inIsolation, skipReason)));
        }

        public static IStepDefinition Enqueue(string prefix, string message, Action body, Action dispose, bool inIsolation, string skipReason)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(prefix, message, body, dispose, inIsolation, skipReason)));
        }
    }
}
