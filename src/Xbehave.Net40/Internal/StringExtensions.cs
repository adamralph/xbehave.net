// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Fluent;

    // TODO: refactor - this isn't right - should be some kind of factory, not extensions
    internal static partial class StringExtensions
    {
        public static IStepDefinition _(this string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(step), inIsolation, skip)));
        }

        public static IStepDefinition _(this string message, Action step, bool inIsolation = false, string skip = null)
        {
            return _(message, DisposableFunctionFactory.Create(step), inIsolation, skip);
        }

        public static IStepDefinition _(this string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return _(message, DisposableFunctionFactory.Create(step), inIsolation, skip);
        }

        public static IStepDefinition _(this string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return _(message, DisposableFunctionFactory.Create(step, dispose), inIsolation, skip);
        }
    }
}
